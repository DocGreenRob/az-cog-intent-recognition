using System.Collections.Generic;

namespace Sample.Models
{
    public class PredictionDto
    {
        public List<EntityDto> entities { get; set; }
        public List<IntentDto> intents { get; set; }
        public string projectKind { get; set; }
        public string topIntent { get; set; }
    }
}
