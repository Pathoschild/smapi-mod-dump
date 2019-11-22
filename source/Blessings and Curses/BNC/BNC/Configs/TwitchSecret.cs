
namespace BNC
{
    class TwitchSecret
    {
        public string _Comment1{ get; set; } = "Note that your token is as good as a password. Do not show this token to anyone not authorized to use your account. ";
        public string _Comment2 { get; set; } = "One way of getting your token is using http://twitchtokengenerator.com and select chatbot. A token may not last forever so you might have to refresh it every now and than.";

        public string Twitch_User_Name { get; set; } = "";

        public string OAuth_Token { get; set; } = "";

        public string Twitch_Channel_Name { get; set; } = "";
    }
}
