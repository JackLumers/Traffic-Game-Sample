# Traffic-Game-Sample
Test task game sample using MVP pattern, UniTasks and Addressable assets.

# Project installation:
- Clone git repo => Enter playmode from **TrafficGameScene**.
- To delete save file go to **TrafficGame/ScriptableObjects/LevelModel** and choose **Delete Save** in context menu. Also choose **Reset** if you need to reset values to default.

# Known bugs:
- Cars can be spawned on top of each other after save load.
- Car that stops right behind car that enters finish zone, don't starts move after.

They are related to OnTriggerEnter/OnTriggerExit calls. Need a workaround for spawned and for disabled/destroyed objects inside a collider with this callback.

Already took a good amount of time on this, so decided to leave it as it is for now. 
MVP pattern not implemented perfectly so I will research to make a better approach and
will fix mentioned bugs later.
