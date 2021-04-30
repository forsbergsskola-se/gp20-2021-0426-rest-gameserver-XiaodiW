using System.Linq;
using Microsoft.Extensions.Configuration;

namespace GitHubExplorer {

    public static class MicroSoftSecretsManager {
        /// <summary>
        /// Returns the saved secrets in Microsoft Secrets Manager.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string LoadSecret(string key) {
            //preparation:
            //1. dotnet add package Microsoft.Extensions.Configuration.UserSecrets
            //   or add the package via NuGet.
            //2. dotnet user-secret init
            //3. dotnet use-secrets set "github-token" "MytokenSecrets"
            //  dotnet user-secrets list
            // dotnet user-secrets clear.
            // ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
            // %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
            var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var secretProvider = config.Providers.First();
            secretProvider.TryGet(key, out var token);
            return token;
        }
    }

}