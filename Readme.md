Gets google analytics data using OAuth 2.0

Settings are saved in root folder of the executable in app.config (beware, these settings are not encrypted)

An active google analytics account is needed. You'll need to download a client_secret.json file from the google api console
(see here: https://developers.google.com/fit/android/get-api-key)

Executable can be scheduled to run every day using windows task scheduler for example

Results will be appended to existing csv file, or if that doesn't exists, one will be created

I've added one custom parameter from google analytics for the use case I needed it for (customVarValue2). You should delete this if you want to use this code for your own project. You can also add your own custom variable.

For more information, see the developer page for google analytics:
https://developers.google.com/analytics/devguides/reporting/core/v4/

If you have questions: feel free to send me a message via twitter @johananvm
