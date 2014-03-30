Overview
========
iTween Multi Path is an extension to iTween Visual Editor Unity plugin.

Please note that you need to download and import iTween Visual Editor from
Unity Asset Store before importing iTween Multi Path.

With iTween Multi Path, you can have your favorite game object or camera follow
multiple successive paths created with iTween Visual Editor.

You can use iTween Multi Path for creating really long paths consisting of multiple
spline curves and spanning large areas in your scene, as well as for cut scene camera
work where you can abruptly switch between different camera paths.

Full color screenshot textures are for illustrative purposes only and are not
included in the example project.

Please visit http://rocketalien.com for more details and for any feedback.

Please contact support@rocketalien.com for any issues.

Instructions
============

1. Download and import iTween Visual Editor plugin from Unity Asset Store.
2. Use iTween Visual Editor to create several iTween paths in your scene.
3. Download and import iTween Multi Path plugin from Unity Asset Store.
4. Add iTweenMultiPath component to your scene game object.
5. Drag the main camera or a game object onto "Actor" property.
6. Drag each path creared with iTween Visual Editor onto "Actor Paths" property.
7. Set "Actor Speed" property which specified the speed of the actor in the scene.
8. Set "Next Path Distance" property which specifies distance to next path if using closest path method.
9. Select appropriate value in "Next Path Method" property dropdown.
   a. "StartNextPath" method simply selects next path from "Actor Paths" when object finishes traversing the current path.
   b. "StartClosestPath" method selects path with the start position closest to the end position of the finished path.
      "StartClosestPath" looks for another path within "Next Path Distance" of the finished path.
10. Click "Play" and see your camera or game object follow multiple successive paths.

Release History
===============

0.1.0.0
-------
Initial release.

0.1.0.1
-------
Updating README.
