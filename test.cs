public class AnalyticsStandartParamsProvider : IStandardParamsProvider
{
    public CloudAnalyticsInfo GetStandardParams(string playerID) => PostLoginStandardParameterProvider.GetCloudAnalyticsInfo();//TODO: Once LuckyVR.VegasInfinite.Analytics becomes its own assembly, eliminate IStandardParamsProvider and call this function directly from GsExecRequest to eliminate needless complexity. 
}
