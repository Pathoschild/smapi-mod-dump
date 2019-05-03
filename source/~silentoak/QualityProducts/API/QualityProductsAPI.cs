namespace SilentOak.QualityProducts.API
{
    public class QualityProductsAPI : IQualityProductsAPI
    {
        public QualityProductsConfig Config { get; }

        public QualityProductsAPI(QualityProductsConfig config)
        {
            Config = config;
        }
    }
}