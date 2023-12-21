# ProgressiveDeadline
Progressive Deadline is a mod crafted to empower players with the ability to customize game deadlines according to their preferences. By offering adjustable scaling options, this mod allows users to modify the pace of the game. Whether aiming for a more relaxed experience or seeking a challenging thrill, this mod grants the flexibility to make the game easier or significantly more demanding based on individual choices.

## WARNING
If you're comming from versions prior to 2.0.0, please delete your config file so that the mod works properly. It will still work if you don't do that, but the deadlines will be weird.

## How does it work?
The mod operates by recalculating the game's original three-day deadline using five key parameters:

- Min Deadline (Default: 2): Defines the minimum duration the game can allocate for completing a quota.
- Max Deadline (Default: a very big number): Sets the upper limit of days within which the quota needs to be accomplished.
- Min Daily Scrap (Default: 100): Establishes the baseline for the amount of scrap theoretically collectible in a day.
- Min Daily Scrap Increase (Default: 30): Determines the incremental increase in the minimum daily scrap value each time a quota is met.
- Min Daily Scrap Increase Steepness(Default: 200): Determines the rate at wich the min daily scrap increase will grow based on the amount of quotas complete.

Taking into account the parameters set by the user, the mod calculates the deadline in the following manner:

- Average Days Calculation: The mod then calculates the average days needed to fulfill the scrap quota using a straightforward formula:
`deadline = quota / minDailyScrap`

- After a quota is complete, it will increase the minimum daily scrap by `minDailyScrapIncrease`, and increase `minDailyScrapIncrease` based on the same algorithm as the quota (you can disable the minDailyScrapIncrease increase in the config file, so the minDailyScrap increases linearly).

This table showcases the deadlines using the default config values (version 2.0.0). You can also use [this tool to calculate your own deadlines](https://lethal-org.github.io/tools/quota-calculator).
| Quota | Daily | Deadline |
|-------|-------|----------|
| 130   | 100   | 2        |
| 217   | 134   | 2        |
| 312   | 167   | 2        |
| 427   | 196   | 3        |
| 631   | 229   | 3        |
| 920   | 259   | 4        |
| 1147  | 292   | 4        |
| 1493  | 324   | 5        |
| 2033  | 357   | 6        |
| 2462  | 411   | 6        |
| 3074  | 456   | 7        |
| 3738  | 506   | 8        |
| 4538  | 550   | 9        |
| 5417  | 605   | 9        |
| 6454  | 633   | 11       |
| 7696  | 691   | 12       |
| 9007  | 760   | 12       |
| 10244 | 837   | 13       |
| 12281 | 934   | 14       |
| 13916 | 1009  | 14       |
| 15350 | 1067  | 15       |


# Installation
1. Install BepInEx.
2. Run your game once with BepInEx installed to generate the necessary folders and files.
3. Move the BepInEx folder from the ProgressiveDeadline folder into your game directory, select replace files if prompted. 

       Alternatively, just move the ProgressiveDeadlines.dll from the ProgressiveDeadline/BepInEx/plugins folder to your game directory's BepInEx/plugins folder. If you do so, run the game once so you can generate the config.
5. Modify the Config LethalOrg.ProgressiveDeadline.cfg within your game directory's BepInEx/config folder and change the settings to what you prefer.
6. You're done!

## Acknowledgements:
- This mod a is fork of @Kraykennn [LC-DynamicDeadline](https://github.com/Kraykennn/LC-DynamicDeadline), I decided to fork it because I ended up rewriting everything from scratch to lean how to mod. Their mod is based on an average daily scrap you actually collected, meaning the game can theoretically go on forever.
