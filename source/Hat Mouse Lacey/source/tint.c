#include <png.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <libgen.h>

/*
 * This program takes two arguments: the first is the path to an 8-bit indexed
 * PNG image containing an asset to be tinted to match various recolors. After
 * reading the image (e.g. 'source/house.png'), it checks for a palettes file
 * alongside it (e.g. 'source/house-palettes.txt'), then reads each palette
 * from that file.
 *
 * For each palette defined there, it updates the palette on the loaded image,
 * then renders the image data out to a new PNG image at 8-bit RGBA depth. The
 * second argument to the program is the output directory where these images
 * will be written. The filenames are constructed based on the original name
 * and the names given in the palettes file.
 *
 * Color 0 in the palette is replaced with a fully transparent pixel; color 1
 * is replaced with black at 50% opacity (for shadows). The palettes in the
 * palettes file do not contain entries for colors 0 and 1; they start with
 * color 2.
 *
 * Each palette in the palettes file is a list of whitespace-separated tokens
 * (any whitespace is acceptable). e.g.:
 *      vanilla spring
 *      56311c 835a38 bc8925 fcbb32 693127 7c3f26 8b4727 99542c
 *      ab680f cc7710 466584 549dbf 56bbc4 936f5e dac190 56311c
 *      7e2c1f a3461a cf5f11 f58716 403430 655345 807547 a29964
 * The first token is the name of the palette, and the second is the variant;
 * these can be any string. They will be reproduced in the output filenames,
 * so please only use filename-safe characters.
 * Each subsequent string is a 6-digit hex representation of an 8-bit RGB
 * color (like CSS, but with no '#'), starting with color 2. It is an error to
 * include too few colors for the source image palette; any palette without
 * enough tokens will be skipped. If the palette has too many colors, the
 * extra colors will not be used (you will see a warning if this happens).
 *
 * The separator token '---' (or end of file) marks the end of a palette.
 * More palettes may follow.
 */

png_color *active_palette;
int palette_size;
char palette_name[32], palette_variant[32];
png_uint_32 width, height;
png_byte **buffer;

int read_png(const char *);
int read_palette(FILE *);
int write_png(const char *);
void transform_fn(png_structp, png_row_infop, png_bytep);


int main(int argc, char **argv)
{
    if (argc < 3) {
        fprintf(stderr, "Usage: %s png-file output-dir\n", argv[0]);
        return 1;
    }

    // get nosuffix before read_png. if done afterward, a baffling extra
    // character appears in some filenames (???)
    int len = strlen(argv[1]);
    char *nosuffix = (char *) malloc(len-3);
    memcpy(nosuffix, argv[1], len-4);
    nosuffix[len-3] = '\0';

    if (read_png(argv[1]) != 0) {
        fprintf(stderr, "Abort.\n");
        return 1;
    }
    char *namepart = basename(nosuffix);
    char *palette_path = (char *) malloc(len+10);
    memcpy(palette_path, argv[1], len-4);
    memcpy(palette_path + len - 4, "-palettes.txt", 13);
    palette_path[len+9] = '\0';
    FILE *palp = fopen(palette_path, "r");
    if (!palp) {
        fprintf(stderr, "No palette file '%s'. Abort.\n", palette_path);
        return 1;
    }
    while (1) {
        int r = read_palette(palp);
        if (r == 2) { // EOF
            break;
        }
        else if (r == 1) { // bad palette
        }
        else if (r == 0) {
            char outfile[PATH_MAX];
            snprintf(outfile, PATH_MAX, "%s/%s_%s_%s.png",
                    argv[2], namepart, palette_name, palette_variant);
            fprintf(stdout, "Writing image to '%s'...", outfile);
            if (write_png(outfile) != 0) {
                fprintf(stdout, "error\n");
                continue;
            }
            fprintf(stdout, "done.\n");
        }
    }
    fclose(palp);
    return 0;
}


int read_png(const char *path)
{
    int code = 0;
    FILE *finp;
    png_structp png;
    png_infop infop;
    unsigned char header[8];
    int len;
    int bit_depth;
    int color_type;
    png_color *t_palette;
    int r;

    finp = fopen(path, "rb");
    if (!finp) {
        fprintf(stderr, "read_png: can't open file '%s' for reading\n", path);
        return 1;
    }
    if (8 != (len = fread(header, 1, 8, finp))) {
        if (feof(finp)) {
            fprintf(stderr, "read_png: data too short\n");
            code = 1;
            goto fin;
        }
    }
    if (0 != png_sig_cmp(header, 0, len)) {
        fprintf(stderr, "read_png: not PNG data\n");
        code = 1;
        goto fin;
    }

    png = png_create_read_struct(
            PNG_LIBPNG_VER_STRING, (png_voidp)NULL, NULL, NULL);
    if (!png) {
        fprintf(stderr, "read_png: could not initialize libpng\n");
        code = 1;
        goto fin;
    }
    infop = png_create_info_struct(png);
    if (!infop) {
        fprintf(stderr, "read_png: could not initialize libpng\n");
        code = 1;
        goto fin;
    }
    if (setjmp(png_jmpbuf(png))) {
        fprintf(stderr, "read_png: error setting up PNG input\n");
        code = 1;
        goto fin;
    }

    png_init_io(png, finp);
    png_set_sig_bytes(png, len);

    png_read_info(png, infop);
    png_get_IHDR(png, infop, &width, &height,
            &bit_depth, &color_type, NULL, NULL, NULL);

    if (color_type != PNG_COLOR_TYPE_PALETTE) {
        fprintf(stderr, "read_png: image is not indexed\n");
        code = 1;
        goto fin;
    }
    if (bit_depth < 8) {
        png_set_packing(png);
    }

    /*
     * have libpng use a stack pointer here, then copy the result to our
     * global. if we read the PLTE directly to the global, then i think
     * libpng frees it when we clean up the png struct and we get some use-
     * after-free behavior (observed: partial clobbering of palette colors).
     */
    png_get_PLTE(png, infop, &t_palette, &palette_size);
    active_palette = (png_color *) malloc(palette_size * sizeof(png_color));
    memcpy(active_palette, t_palette, palette_size * sizeof(png_color));

    buffer = png_malloc(png, height * sizeof(png_byte*));
    size_t bc = png_get_rowbytes(png, infop);
    for (r = 0; r < height; ++r) { 
        buffer[r] = png_malloc(png, bc);
    }
    if (setjmp(png_jmpbuf(png))) {
        fprintf(stderr, "read_png: error reading image data\n");
        code = 1;
        goto fin;
    }

    png_read_image(png, buffer);

fin:
    if (infop && png) {
        png_destroy_read_struct(&png, &infop, (png_infopp)NULL);
    }
    else if (png) {
        png_destroy_read_struct(&png, (png_infopp)NULL, (png_infopp)NULL);
    }
    if (finp) {
        fclose(finp);
    }
    return code;
}


int read_palette(FILE *stream)
{
    if (feof(stream)) {
        return 2;
    }
    int rv = 0;
    int code;
    int count = 2;
    int extra = 0;
    char token[16];
    int r, g, b;
    if ((code = fscanf(stream, "%31s", palette_name)) == EOF) {
        return 2;
    }
    if ((code = fscanf(stream, "%31s", palette_variant)) == EOF) {
        return 2;
    }
    while (count < palette_size) {
        if ((code = fscanf(stream, "%15s", token)) == EOF) {
            fprintf(stderr, "error: out of tokens in palette '%s_%s'\n",
                    palette_name, palette_variant);
            return 2;
        }
        if (strcmp(token, "---") == 0) {
            fprintf(stderr, "error: not enough colors in palette '%s_%s'"
                    " (needed %d, found %d)\n", palette_name, palette_variant,
                    palette_size-2, count-2);
            return 1;
        }
        if (sscanf(token, "%02x%02x%02x", &r, &g, &b) != 3) {
            fprintf(stderr, "error: unparseable color found in palette '%s_%s'\n",
                    palette_name, palette_variant);
            rv = 1;
            break;
        }
        active_palette[count].red = r;
        active_palette[count].green = g;
        active_palette[count].blue = b;
        ++count;
    }
    while ((code = fscanf(stream, "%s", token)) != EOF && strcmp(token, "---") != 0) {
        ++extra;
    }
    if (extra > 0) {
        fprintf(stderr, "warning: ignoring %d extra color%s in palette '%s_%s'\n",
                extra, (extra > 1 ? "s" : ""), palette_name, palette_variant);
    }
    return rv;
}


int write_png(const char *path)
{
    int code = 0;
    FILE *foutp;
    png_structp png;
    png_infop infop;

    foutp = fopen(path, "wb");
    if (!foutp) {
        fprintf(stderr, "write_png: can't open file '%s' for writing\n", path);
        return 1;
    }

    png = png_create_write_struct(
            PNG_LIBPNG_VER_STRING, (png_voidp)NULL, NULL, NULL);
    if (!png) {
        fprintf(stderr, "write_png: could not initialize libpng\n");
        code = 1;
        goto fin;
    }
    infop = png_create_info_struct(png);
    if (!infop) {
        fprintf(stderr, "write_png: could not initialize libpng\n");
        code = 1;
        goto fin;
    }
    if (setjmp(png_jmpbuf(png))) {
        fprintf(stderr, "write_png: error writing PNG output\n");
        code = 1;
        goto fin;
    }

    png_init_io(png, foutp);
    png_set_IHDR(png, infop, width, height,
            8 /*bit depth*/, PNG_COLOR_TYPE_RGB_ALPHA,
            PNG_INTERLACE_NONE, PNG_COMPRESSION_TYPE_DEFAULT,
            PNG_FILTER_TYPE_DEFAULT);
    png_set_sRGB(png, infop, PNG_sRGB_INTENT_PERCEPTUAL);
    png_write_info(png, infop);
    png_set_write_user_transform_fn(png, transform_fn);
    png_write_image(png, buffer);
    png_write_end(png, NULL);

fin:
    if (infop && png) {
        png_destroy_read_struct(&png, &infop, (png_infopp)NULL);
    }
    else if (png) {
        png_destroy_read_struct(&png, (png_infopp)NULL, (png_infopp)NULL);
    }
    if (foutp) {
        fclose(foutp);
    }
    return code;
}

void transform_fn(png_structp png, png_row_infop row_info, png_bytep data)
{
    png_bytep dp;
    int idx;
    int p;
    if (png == NULL) {
        return;
    }
    dp = data;
    for (p = row_info->width-1; p >= 0; --p) {
        idx = dp[p];
        if (idx <= 1) {
            dp[p*4+0] = 0;
            dp[p*4+1] = 0;
            dp[p*4+2] = 0;
            dp[p*4+3] = (idx == 0 ? 0x00 : 0x80);
            continue;
        }
        dp[p*4+0] = active_palette[idx].red;
        dp[p*4+1] = active_palette[idx].green;
        dp[p*4+2] = active_palette[idx].blue;
        dp[p*4+3] = 0xff;
    }
}

