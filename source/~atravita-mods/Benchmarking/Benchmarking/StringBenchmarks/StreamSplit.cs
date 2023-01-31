/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.StringHandler;

namespace Benchmarking.StringBenchmarks;
public class Splitter
{

    private const string lori =
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit.Etiam ullamcorper nunc in nulla bibendum, id dignissim magna egestas.Suspendisse sit amet nulla ac ipsum vestibulum dictum.In aliquam lorem lacinia odio tristique, a dapibus nulla volutpat. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Maecenas interdum tincidunt imperdiet. Curabitur ut lobortis nisi. Morbi luctus vestibulum lectus eu fringilla. Nullam laoreet suscipit velit, a tempor leo fringilla sed.Sed varius elit non eros malesuada sollicitudin.Vivamus feugiat quam et quam ultrices, sed ultricies nisl faucibus. Donec maximus nisl lorem, ac elementum tortor gravida nec.Sed rhoncus mollis arcu, eget auctor mauris ultricies in. Nullam elit erat, mollis id leo id, dignissim convallis nunc.Curabitur tempor tristique turpis sed pretium. Pellentesque nisl purus, interdum quis nunc nec, congue semper odio."
        + " Integer et sagittis arcu. Morbi tempor, justo quis luctus auctor, lorem nibh auctor enim, luctus hendrerit quam tellus sed nibh. Etiam et velit et ante commodo laoreet vitae vitae purus. Mauris elementum justo eros, id finibus augue tincidunt at.Sed sit amet nunc bibendum sapien sollicitudin scelerisque sit amet eu sem. Quisque vitae convallis purus. Phasellus sed laoreet massa. Praesent rutrum varius diam, et porta ligula imperdiet non.Donec ut arcu vel ligula imperdiet faucibus sit amet id leo.Etiam at risus neque."
        + " Morbi et lectus nisl. Donec sit amet tempus orci, sed interdum massa.Fusce et metus non felis pulvinar gravida.Quisque ex neque, commodo vel ultricies eu, mollis ac sem.Maecenas sit amet metus quis sapien tempor luctus eget ac arcu.Nunc erat elit, euismod eget dapibus vel, rutrum non ligula.Quisque feugiat gravida erat at feugiat. Etiam vel tempus nisi. Sed vel sodales ligula, et pharetra massa.Nullam tempor pretium purus, non imperdiet augue condimentum non."
        + " Nam bibendum hendrerit urna, sed vehicula diam varius et.Integer gravida venenatis libero sit amet maximus.Etiam tempor, lectus eget mattis interdum, arcu lacus accumsan nibh, vitae sagittis neque magna non tortor. Morbi id leo gravida dolor bibendum pretium eget vitae sapien. Nulla facilisi. Nulla at lorem id nisl maximus pulvinar.Donec venenatis sem ut risus scelerisque pellentesque.Pellentesque metus sapien, venenatis scelerisque justo sit amet, hendrerit congue nisl.Ut sed turpis magna."
        + " Ut vehicula nibh quis malesuada efficitur. Nunc volutpat nibh sed mi laoreet semper.Nullam nec erat risus. Morbi ac metus sed velit imperdiet dignissim a eget nisi. Donec in interdum enim, maximus condimentum neque.Mauris sollicitudin, velit in pellentesque fringilla, libero augue lacinia ex, id congue eros lorem ac tellus. Cras convallis sagittis risus vitae pulvinar. Morbi scelerisque fringilla diam, eget laoreet ligula.Suspendisse sed lectus aliquam, efficitur tortor eu, placerat elit. Nam vehicula nunc nec ex facilisis feugiat.Phasellus aliquam, magna eu eleifend lobortis, nunc massa scelerisque libero, eget convallis lectus risus congue purus. Vestibulum sem nunc, mattis vel velit eu, molestie pharetra ex.Cras egestas scelerisque porttitor. Aenean et est et tortor porta fermentum.Cras lobortis tortor purus, aliquam hendrerit felis luctus pulvinar.Quisque eu urna eget velit consequat varius non nec sapien.";

    //[Benchmark]
    public void Split()
    {
        foreach (var s in lori.Split())
        {
        }
    }

    //[Benchmark]
    public void StreamSplit()
    {
        foreach (var s in new StreamSplit(lori.AsSpan()))
        {
        }
    }
}
