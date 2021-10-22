using FRS_API.Contracts;
using FRS_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRS_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IDBService dbService;
        public UserController(IDBService _dbService)
        {
            dbService = _dbService;
        }
        [HttpPost("")]
        public async Task<IActionResult> CreateUserAsync([FromBody] User user)
        {
            if (user == null || String.IsNullOrEmpty(user?.Name) || String.IsNullOrEmpty(user?.AzurePersonId))
            {
                return BadRequest("User cannot be null");
            }
            var newUser = await dbService.AddUserAsync(user);
            return Created("", newUser);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await dbService.GetAllUsersAsync().ConfigureAwait(false);
            return Ok(users);
        }

        [HttpGet("Face")]
        public async Task<IActionResult> IsUserAuthenticated(int userId, string azurePersonId)
        {
            var authenticated = await dbService.IsUserAuthenticated(userId, azurePersonId).ConfigureAwait(false);
            if (authenticated)
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("Voice")]
        public async Task<IActionResult> AuthenticateVoice(int userId)
        {
            // TO-DO Save Audio received to C://Temp.wav 
            string subscriptionKey = "b23d4db8df61461883a908492c8f238b";
            string region = "westus";
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            using (var client = new VoiceProfileClient(config))
            {
                var all = await client.GetAllProfilesAsync(VoiceProfileType.TextIndependentVerification);
                //TO-DO  Get Voice Profile basd on USerId from the above list
                var voiceId = await dbService.GetUserVoiceId(userId).ConfigureAwait(false);

                var userProfile = all.Where(v => v.Id == voiceId).SingleOrDefault();

                var success = await SpeakerVerify(config, userProfile != null ? userProfile : all[0]);
                if (success)
                    return Ok();
            }
            return Unauthorized();
        }

        public static async Task<bool> SpeakerVerify(SpeechConfig config, VoiceProfile profile)
        {
            try
            {

                var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromWavFileInput("F://Temp.wav"));
                var model = SpeakerVerificationModel.FromProfile(profile);

                Console.WriteLine("Speak the passphrase to verify: \"My voice is my passport, please verify me.\"");
                var result = await speakerRecognizer.RecognizeOnceAsync(model);
                Console.WriteLine($"Verified voice profile for speaker : {result.ProfileId}, score is {result.Score} " +
                    $",Reason : {result.Reason},Id : {result.ResultId}");
                return result?.Score > 0.5;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;

            }
        }

    }
}
