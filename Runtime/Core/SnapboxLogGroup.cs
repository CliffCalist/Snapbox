using System.Collections.Generic;
using System.Text;

namespace WhiteArrow.SnapboxSDK
{
    public class SnapboxLogGroup
    {
        private readonly string _header;
        private readonly Queue<string> _logs = new();


        public bool HasError { get; private set; }



        private const string HEADER_FORMAT = "[" + SERVICE_NAME + "]-[{0}]";
        private const string SERVICE_NAME = "Snapbox";



        public SnapboxLogGroup(string header)
        {
            _header = header;
        }



        public void AddLog(string message)
        {
            _logs.Enqueue(message);
        }

        public void AddError(string message)
        {
            _logs.Enqueue(message);
            HasError = true;
        }



        private string BuildGroupMessage(string header, Queue<string> messages)
        {
            var sb = new StringBuilder();
            sb.AppendLine(header);

            while (messages.Count > 0)
            {
                var message = _logs.Dequeue();
                sb.AppendLine($" • {message}");
            }

            return sb.ToString();
        }



        public override string ToString()
        {
            var message = string.Format(HEADER_FORMAT, _header) + "\n";
            message = BuildGroupMessage(_header, _logs);
            return message;
        }
    }
}