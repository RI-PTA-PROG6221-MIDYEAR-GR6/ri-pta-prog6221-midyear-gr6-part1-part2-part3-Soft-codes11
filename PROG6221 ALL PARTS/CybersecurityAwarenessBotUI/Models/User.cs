namespace CybersecurityAwarenessBot.Models
{
    /// <summary>
    /// Represents the person chatting with the bot. Doubles as the bot's "memory":
    /// it stores the name, favourite topic, last mood and topics explored so far.
    /// </summary>
    public class User
    {
        public string Name { get; set; }

        /// <summary>Topic key (e.g. "privacy") or free text if the topic is not one the bot knows.</summary>
        public string FavouriteTopic { get; set; }

        /// <summary>The most recent sentiment detected (e.g. "Worried").</summary>
        public string LastSentiment { get; set; }

        /// <summary>Keys of the topics the user has discussed, in order, without duplicates.</summary>
        public List<string> TopicsDiscussed { get; } = new();

        public User(string name)
        {
            Name = name;
        }

        public void RecordTopic(string topicKey)
        {
            if (!TopicsDiscussed.Contains(topicKey))
            {
                TopicsDiscussed.Add(topicKey);
            }
        }
    }
}
