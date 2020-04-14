namespace EiTK.Utils
{
    public class GuiUtils
    {
        public static bool isHovered(int mouseX, int mouseY, int x, int y, int width, int height)
        {
            return mouseX >= x && mouseX - width <= x && mouseY >= y && mouseY - height <= y;
        }
    }
}