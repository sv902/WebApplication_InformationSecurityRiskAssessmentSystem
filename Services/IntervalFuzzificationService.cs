namespace WebApplication_InformationSecurityRiskAssessmentSystem.Services
{
    public class IntervalFuzzificationService
    {
        public void FuzzifyIntervals(List<double[]> intervals)
        {
            double CF = DetermineClosenessFactor(); // Етап 1
            List<double> medians = DetermineMedians(intervals); // Етап 2
            double SP = DetermineShiftParameter(medians[0], intervals[0], intervals[1], CF); // Етап 3
            double SC = DetermineStretchingCoefficient(medians[medians.Count - 1], intervals[intervals.Count - 1], intervals[intervals.Count - 2], SP, CF); // Етап 4

            List<double[]> fuzzifiedIntervals = FormFuzzyNumbers(medians, intervals, SP, SC, CF);// Етап 5

            intervals.Clear();
            intervals.AddRange(fuzzifiedIntervals);
            // Робота з отриманими нечіткими числами
            foreach (var fuzzifiedInterval in fuzzifiedIntervals)
            {
                Console.WriteLine($"[{fuzzifiedInterval[0]:F2}; {fuzzifiedInterval[1]:F2}; {fuzzifiedInterval[2]:F2}; {fuzzifiedInterval[3]:F2}]");
            }
        }

        private double DetermineClosenessFactor()
        {
            return 0.25;
        }

        private List<double> DetermineMedians(List<double[]> intervals)
        {
            // Етап 2
            List<double> medians = new List<double>();
            foreach (var interval in intervals)
            {
                double median = (interval[0] + interval[1]) / 2;
                medians.Add(median);
            }
            return medians;
        }

        private double DetermineShiftParameter(double median, double[] interval, double[] nextInterval, double CF)
        {
            // Етап 3
            return median - CF * (nextInterval[0] - interval[0]);
        }

        private double DetermineStretchingCoefficient(double lastMedian, double[] lastInterval, double[] previousInterval, double SP, double CF)
        {
            // Етап 4
            return lastInterval[1] / (lastMedian + CF * (lastInterval[1] - lastInterval[0]) - SP);
        }

        // Етап 5
        private List<double[]> FormFuzzyNumbers(List<double> medians, List<double[]> intervals, double SP, double SC, double CF)
        {
            List<double[]> fuzzyNumbers = new List<double[]>();

            for (int i = 0; i < medians.Count; i++)
            {
                double ai, b1i, b2i, ci;

                b1i = SC * (medians[i] - CF * (intervals[i][1] - intervals[i][0]) - SP); // Перша вершина і-того терму
                b2i = SC * (medians[i] + CF * (intervals[i][1] - intervals[i][0]) - SP); // Друга вершина і-того терму

                if (i == 0)
                {
                    ai = 0;
                }
                else
                {
                    ai = SC * (medians[i - 1] + CF * (intervals[i - 1][1] - intervals[i - 1][0]) - SP);
                }

                if (i == medians.Count - 1)
                {
                    ci = intervals[i][1]; // максимальне значення останнього інтервалу
                }
                else
                {
                    ci = SC * (medians[i + 1] - CF * (intervals[i + 1][1] - intervals[i + 1][0]) - SP);
                }

                fuzzyNumbers.Add(new double[] { ai, b1i, b2i, ci });
            }

            return fuzzyNumbers;
        }
    }
}
