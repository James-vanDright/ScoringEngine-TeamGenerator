# ScoringEngine-TeamGenerator
## About
The Team Generator is a specific tool designed only to take in a file from a Scoring Engine (https://github.com/scoringengine/scoringengine).
## Which File?
The specific file required is the competition.yaml(https://raw.githubusercontent.com/scoringengine/scoringengine/master/bin/competition.yaml) file, which stores the configuration data for each team in a competition. A more indepth analysis of its use can be found on their github, or their documentation (https://scoringengine.readthedocs.io/en/latest/)
## What to do
### Step 1: Create a file with only 1 team.
From the competition file, create a file populated with only 1 team. The ```---\nteams:``` at the top of the old file should not be present in the new file.
### Step 2: Open the program
Once open, click the button in the center. This will prompt you to select your template file that you created in the previous step.
Afterwards, choose the amount of teams you wish to have.
### Step 3: Edit the teams
The program will generate the desired teams for you. Besides specific values, such as team names and service IP addresses, the values that are changed on one team should be changed synchronously with all teams. This process should save most of the manual work someone would have to do to add new services and make adjustments to old ones, as well as having to reflect those changes amongst all teams.
### Step 4: Save the new file
Hover over "File" and click "Save As". Select the desired save location from the prompt, and it should be ready to use.
