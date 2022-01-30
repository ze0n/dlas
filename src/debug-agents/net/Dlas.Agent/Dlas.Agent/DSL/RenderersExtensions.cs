namespace Debug.Like.A.Scientist
{
    public static class RenderersExtensions
    {
        public static ScalarReporter AsTimeSeries(this ScalarReporter reporter)
        {
            reporter.RepresentationInfo.AsChartType = Chart.TimeSeries;
            reporter.RepresentationInfo.X = ValueRepresentationInfo.AxisTimestamp;
            reporter.RepresentationInfo.Y = ValueRepresentationInfo.AxisValue;
            return reporter;
        }
        
        public static ScalarReporter AsHistogram(this ScalarReporter reporter)
        {
            reporter.RepresentationInfo.AsChartType = Chart.Histogram;
            reporter.RepresentationInfo.X = ValueRepresentationInfo.AxisValue;
            return reporter;
        }

        //public static ScalarReporter AsBarChart(this ScalarReporter reporter, yaxis)
        //{
        //    reporter.RepresentationInfo.AsChartType = Chart.TimeSeries;
        //    reporter.RepresentationInfo.X = ValueRepresentationInfo.AxisTimestamp;
        //    reporter.RepresentationInfo.Y = ValueRepresentationInfo.AxisValue;
        //    return reporter;
        //}

        //public static ReporterBase UsingRenderer(this ReporterBase reporter, RendererType renderer)
        //{
        //    reporter.Context.Renderer = renderer;
        //    return reporter;
        //}
    }
}
