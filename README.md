# Unity-All-About-Time
A simple Unity project to play around with time related features of Unity. This project is intended for beginner / intermediate developers to learn about Unity. Covers Time class, RigidbodyInterpolation, QualitySettings, VSync and more.

I've published an in depth blog post about time in Unity and this is the accompanying project for that post. If you are familiar with Unity already, you can dive right into the project. If this readme file and the project seem confusing, feel free to read the blog post.
https://medium.com/@nosuchstudio/all-about-time-in-unity-game-engine-4c290d2772c7

On the left side there are controls to change Unity parameters (listed and explained below). On the right side you'll see some stats such as FPS, physics updates per second, real time, game time and screen's refresh rate.

You can access the web build of the project here: https://hk1ll3r.github.io/Unity-All-About-Time/

![screenshot](https://raw.githubusercontent.com/hk1ll3r/Unity-All-About-Time/master/screenshot.png)

The parameters:

* **Application.targetFrameRate**

  How many frames we want rendered per second of real time. -1 is a special value and is different on different platforms.

* **Time.FixedDeltaTime**

  The interval at which we want physics step to happen in game time. 0.02 means 50 updates per second of game time.

* **Time.timeScale**

  If CaptureDeltaTime == 0: time scale for game time compared to real time.  
  If CaptureDeltaTime >  0: time scale for game time compared to capture time.
  
* **Time.CaptureDeltaTime**

  If non-zero, game time advances with each frame rendered, rather than with real time.  
  For example if CaptureDeltaTime = 0.02, each frame rendered advances game time by (0.02 * timeScale) seconds.

* **VSyncCount**

  Overrides Application.targetFrameRate and renders frames in sync with the screen's refresh rate.

* **RigidbodyInterpolation**

  If set to None, the Rigidbody's pose will not change between physics steps.  
  If set to Interpolate or Extrapolate, its pose will change in Update() calls between physics steps.
  
* **Quality Level**

  The quality level of the game.
  
* **Balls**

  How many balls to have in the scene.
  
* **FixedProcessingTime**

  How long the call to FixedUpdate() should take in real time? Use this to simulate a physics heavy game in which physics steps take time. Uses Thread.Sleep for simulation.
  
* **FrameProcessingTime**

  How long the call to Update() should take in real time? use this to simulate a graphics-heavy game in which rendering frames takes considerable amount of time. Uses Thread.Sleep for simulation.

