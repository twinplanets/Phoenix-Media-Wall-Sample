# Introduction

This project is designed to assist artists with creating content for the media wall at Phoenix Cinema and Arts Centre, Leicester, UK. They are split into two sections, the Unity 2022(LTS) Sample, and the Unity 2022(LTS) Package. You can find the documentation for the Javascript web sample here: https://github.com/PhoenixDigiArt/media-wall-webapp-sample.

In this repository, there are two options for your project, the Media-Wall Sample source code and the Media Wall Package (Found in the GitHub upm branch), and as the name suggests, the sample includes sample project. In contrast, the package installs the required functionality and the samples into your own project.

This document should be used as a guide on using the Media-Wall plugin for Unity, which can be downloaded through the Unity package manager. To learn about Unity, please refer to the Unity Learn and Unity Documentation web pages for guidance.

### Recommendations

The Phoenix-Media-Wall package was built for Unity version 2022(LTS) and it is recommended that you use this version. All other Unity versions are untested and there may be unexpected errors with no documented solutions if you choose to use a different Unity build.

The bodytracking in this sample expects the 3D model used to have a humanoid skeleton. https://www.mixamo.com/ is an excellent resource for downloading pre-rigged models for free, and it also has a tool for automated rigging.

# Installation

To install this package, it is necessary to install WebGL support to your Unity installation. Either Install Unity2022(LTS) with the WebGL module in the Unity Hub launcher, or update your current installation of Unity2022(LTS) to include the WebGL module.

If you are using Unity 2022, but the subversion isn't 2022.3.6f1, they you will still be prompted before launching about whether you want to change the project version. This is okay, as long as you are using a 2022(LTS) version it should work. LTS means Long-Term support, so the version receives regular bug fixes that shouldn't effect the package.
![Screenshot](Docs/install.png)

### Dependancies

The Unity Project depends on the Native Websocket API package by Endal on GitHub, this package can be added to your project using the Unity Package Manager. Open the Package Manager window, Press the + in the upper left and click and from Git URL. The URL is: https://github.com/endel/NativeWebSocket.git#upm

INFORMATION: Downloading this package through the Unity Package Manager (Refer to Install option 1) installs this for you, you only need to ensure you have it installed when downloading the sample project instead of the package.

![Screenshot](Docs/packagemanagergit.png)

## Install Option 1 (RECOMMENDED): Installing the Package using the Unity Package Manager

Once in your Unity project, open Window > Package Manager (Located on the top menu bar). Then, in the package manager, click the Plus icon in the top right of the window and click Add package by git URL... The URL to Enter is:

https://github.com/twinplanets/Phoenix-Media-Wall-Unity-SDK.git#upm

Once added, the package should install. Once installed, the package installer window should appear. If not, navigate to 'Window > Show Phoenix SDK Installer' to open the installer manually.

First click the Install Native Websockets button and wait for it to install. Unity may give you critical errors during the install, if these havent disappeared within a few minutes, try reopening your project.

Secondly, click the Install WebGLTemplate and Install StreamingAssets button. These add the template to build the app for the media wall, and the debug data for the bodytracking to the assets folder.

Finally, you can close the installer. In the package manager, you can install two sample scenes that use the bodytracking and the webcam, along with a set of scene prefabs that set out a template scene.

## Install option 2: Opening the Media Wall Sample

Either use Git to clone the repository or download the source code from the Github page. Once downloaded, open Unity Hub and open the Media-Wall Sample project in Unity2022(LTS) with WebGL support.

# Quick-Start Guide

Once you have your scene set-up, you are free to use Unity to develop your scene. Make sure you develop everything to work with the WebGL player of Unity, as it has different requirements to standalone development. The Unity Documentation and Forums have extensive resources on what can and can't be done in WebGL. To access the bodytracking data, you have to create a gameobject with an instance of the Motion Object class, and include it in the WsClient's list of game objects.

### Creating a bodytracking scene

Once the package is installed, you need to create a scene to set-up the bodytacking. First create and open a new scene and create an empty object. Name this empty object 'Systems'. This object will be the parent for all the important systems required. The image below is an example of an organised Hierarchy for a media-wall project.

![Screenshot](Docs/hierarchy.png)

Under the 'Systems' object (Name is unnecessary, call it what you want), create a new gameobject as a child. This should be named 'WebSocket Client', as once the WsClient script is added, this object will handle updating all the relevent scripts that require the bodytracking data.

![Screenshot](Docs/websocketclient.png)

The websocket client class has a list of 'Motion Objects', these are the different types of classes that use the motion data provided by the media-wall bodytracking data. In the image above, we can see the scene has two motion object renderers, one for the debug data and one to animate a character avatar. This is done to compare the data between the complex character and the raw data. More information on motion objects can be found below.

There is also the option to add a text object, this simply reads out the data and applies it to a text element, and it can be left empty if undesired.

Finally, there is the Sample Data Stream button. If checked, the Ws Client will instantiate a class that transmits sample data. This is to enable development remotely with no access to the live bodytracking. If the WsClient loads the sample data, the project has to reload to connect to the genuine connection.

#### Scene Tip

In the prefab folder, there is a folder containing everything needed named Scene. In this folder you can find everything needed for bodytracking already set-up. For instance, you can see the rotation of the ground plane and camera in the Rendering and Level prefab. This is to compensate for the angle that the Zed2 camera is placed at.

### Motion Object

The Motion Object class reads the bodytracking data and instantiates a Skeleton for each collection of joints in the data. If multiple Motion Objects are referenced in the WsClient, the Skeletons will be duplicated for each system.

The Motion Object class on its own is fairly useless, however several classes inherit from it and override some functions.

By overriding the `protected virtual void UpdateMotionObject()` , functionality can be added to the motion object. An example of this can be found in the DebugMotionObject class, which instantiates cubes and spheres at each joint position and updates every frame.

Either use one of the following classes that overrides Motion Object, or create your own. It is not recommended to edit any of the scripts in this package, and if you need to do so, it is recommended to instead copy a script and rename it via file name and class definition. Then you can make any tweaks you like without damaging the code base.

#### Debug Skeletons

The debug motion object class in this package creates simple gameobjects to represent the bodytracking data. Once added to the Websocket Client Motion Objects array, the Debug Motion Object class will instantiate each skeleton in the bodytracking data. This is useful as a comparison tool to ensure that the bodytracking data is being accurately represented. (Tip: Rendering your character as 50% transparent in the material properties allows you to see the debug skeleton joints when they render inside the character body)

![Screenshot](Docs/debugmotionobject.png)

INFORMATION: The debug data was recorded with the camera at a different angle, so it will not line up with the prefab scene. This is okay, as when tested on the media wall you will see it lines up correctly.

#### AvatarIK skeleton

The AvatarIK Motion Object takes a Humanoid Character, an example from Mixamo is already included in the project. The character has a humanoid avatar that the Avatar IK script manipulates. First import your character, select the character, in the inspector select Rig, then set the animation type to Humanoid and the definition either create from this model, or copy from another avatar (Use the Avatar that comes with the included mannequin).

![Screenshot](Docs/avatar.png)

In the samples folder, the Avant-Now examples uses a background image and a custom script, the Rainbow AvatarIK Skeleton. This script applies a random material to each skeleton, changing the colours.

#### Webcam Passthrough

To add the webcam footage to your scene, either as a fullscreen UI element or as a 3d object in the world space, add the Webcam Manager script to an empty gameobject, then drag in each Renderer (3D Objects) and each Raw Image (UI) into the arrays in the Webcam Manager Inspector fields. Enable the cycle camera options and adjust the delay to cycle through all the cameras available on the device. This is for debugging and finding out what index you should use to test at home.

## The device index value represents the webcam device to access. The media wall has multiple, and the index values refer to;

- Anker PowerConf C300 = Anker Webcam
- ZED 2 = Stereo Output
- Blackmagic WDM Capture = Blackmagic Capture Card EG The Mac
- Decklink Video Capture = The Webcam

To get the webcam to work on the Media Wall, the 'Require User Authentication' box needs to be unticked. However, you may find that to access the webcam on your device, it needs to be checked.

The debug device text field sets the text value of a text box to the name of the currently selected device. Useful for debugging.

Setting the X Scale to -1 flips the image horizontally.

![Screenshot](Docs/webcamprefab.png)

### Building for web

First, open the build settings window and ensure that WebGL is the selected platform. Also, include the scene you want the WebGL player to open and add it to the list of Scenes to build. Once completed, select 'Player Settings...'.

![Screenshot](Docs/buildsettings.png)

Once in the player settings, there is a bunch of build settings. Check the Unity Docs for how to set up things like your own logo on the splash screen, or your company name in the metadata. However, the following is necessary;

Under Icon, set the resolution to 1920x1080, Select run in background, and select the custom fullscreen template that is included in the project.

![Screenshot](Docs/icon.png)

Under Other settings, Reduce the stripping level, allow 'unsafe' code, and overall match the settings to the image below.

![Screenshot](Docs/playerother.png)

Finally, in publishing settings, set Enable Exceptions to Full without Stacktrace, the Compression format to Gzip, turn off Data caching, and enable the Decompression Fallback.

![Screenshot](Docs/publish.png)

If all is done correctly, 'build and run' will create a Html webpage that when hosted, will load your scene and play. On the media wall it will access the websocket and collect the motion data. It is recommended to leave sample data stream on so this can be tested on your own machine.

Hint: The WebGL player will only work when hosted, so it shouldn't work by double click. Github Pages is a really simple way to host your webpage.
