# ProgressiveDeadline
Progressive Deadline is a mod crafted to empower players with the ability to customize game deadlines according to their preferences. By offering adjustable scaling options, this mod allows users to modify the pace of the game. Whether aiming for a more relaxed experience or seeking a challenging thrill, this mod grants the flexibility to make the game easier or significantly more demanding based on individual choices.

## How does it work?
The mod operates by recalculating the game's original three-day deadline using four key parameters:

- Min Deadline (Default: 2): Defines the minimum duration the game can allocate for completing a quota.
- Max Deadline (Default: a very big number): Sets the upper limit of days within which the quota needs to be accomplished.
- Min Daily Scrap (Default: 200): Establishes the baseline for the amount of scrap theoretically collectible in a day.
- Min Daily Scrap Increase (Default: 30): Determines the incremental increase in the minimum daily scrap value each time a quota is met.

Taking into account the parameters set by the user, the mod calculates the deadline in the following manner:

- Average Days Calculation: The mod then calculates the average days needed to fulfill the scrap quota using a straightforward formula:
`deadline = quota / minDailyScrap`

- After a quota is complete, it will increase the minimum daily scrap by 
`minDailyScrapIncrease`

Using the default values, this is an example of the deadlines on certain quota values:
| Quota | Daily | Days |
|-------|-------|------|
| 200   | 200   | 2    |
| 500   | 230   | 3    |
| 800   | 260   | 4    |
| 1100  | 290   | 4    |
| 1400  | 320   | 5    |
| 1700  | 350   | 5    |
| 2000  | 380   | 6    |


# Installation
1. Install BepInEx.
2. Run your game once with BepInEx installed to generate the necessary folders and files.
3. Move the BepInEx folder from the ProgressiveDeadline folder into your game directory, select replace files if prompted. 

       Alternatively, just move the ProgressiveDeadlines.dll from the ProgressiveDeadline/BepInEx/plugins folder to your game directory's BepInEx/plugins folder. If you do so, run the game once so you can generate the config.
5. Modify the Config LethalOrg.ProgressiveDeadline.cfg within your game directory's BepInEx/config folder and change the settings to what you prefer.
6. You're done!

## Acknowledgements:
- This mod a is fork of @Kraykennn [LC-DynamicDeadline](https://github.com/Kraykennn/LC-DynamicDeadline), I decided to fork it because I ended up rewriting everything from scratch to lean how to mod.
