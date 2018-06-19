namespace ReQube.Models
{
    public class SonarQubeSeverity
    {
        private SonarQubeSeverity(string value)
        {
            Value = value;
        }

        public static SonarQubeSeverity Blocker => new SonarQubeSeverity("BLOCKER");

        public static SonarQubeSeverity Critical => new SonarQubeSeverity("CRITICAL");

        public static SonarQubeSeverity Info => new SonarQubeSeverity("INFO");

        public static SonarQubeSeverity Major => new SonarQubeSeverity("MAJOR");

        public static SonarQubeSeverity Minor => new SonarQubeSeverity("MINOR");

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
