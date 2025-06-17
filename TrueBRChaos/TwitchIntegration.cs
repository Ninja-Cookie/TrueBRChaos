/*using TwitchLib.Api.Helix.Models.Polls.CreatePoll;
using TwitchLib.Unity;

namespace TrueBRChaos
{
    internal class TwitchIntegration
    {
        public void Init()
        {
            Api api = new Api();

            api.Settings.ClientId = "zdx1ezslw56t22nng56jyezslgbzqo";
            api.Settings.AccessToken = "bn4ix085exp2qafebyuqfnv4nbqzz1";

            CreatePollRequest createPollRequest = new CreatePollRequest() { Title = "Test Poll" };
            createPollRequest.Choices = new Choice[]
            {
                new Choice() { Title = "Option 1" },
                new Choice() { Title = "Option 2" }
            };

            api.Helix.Polls.CreatePollAsync(createPollRequest);
        }
    }
}*/

namespace TrueBRChaos
{
    internal class TwitchIntegration
    {
        public void Init()
        {
        }
    }
}