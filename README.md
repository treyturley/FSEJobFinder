# Project Info
Welcome to FSE Job Finder. This project started as a simple C# console app that would pull job and airplane data from the FSEconomy API. The project stayed like this until I started exploring full stack development. Now an ASP.NET Web API called [FSEDataFeedAPI](https://github.com/treyturley/FSEJobFinder/tree/dev/FSEJobFinder/FSEDataFeedAPI) has been added along with a small [React frontend](https://github.com/treyturley/fsejobfinder-react) which makes it easy to quickly check for available jobs.

The app is currently hosted at https://treyturley.com/fse-job-finder

## How to Retrieve Jobs
The application relies on the FSEconomy Data Feeds which require an active account and access key to use. Please check the [FSEconomy User Guide](https://sites.google.com/site/fseoperationsguide/data-feeds) for details on getting started with FSEconomy and getting your access key.

Once you have an access key, it can be entered in the top right of the FSE Job Finder site and saved for return visits. Then you will be able to select an airplane type and get available assignments for it.
