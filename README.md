# Principles of Game Audio and Sound Design – 2nd Edition

This repository features the supporting project for the 2nd edition of "Principles of Game Audio and Sound Design," by [Jean-Luc Sinclair](https://college.berklee.edu/faculty/jean-luc-sinclair) and [David Cardona](https://linktr.ee/dcbapc), published by [Routledge](https://www.routledge.com/).

You may find the book's main website . Errata and addenda are available on the book's companion website, which you may find [here](https://principlesgameaudioandsounddesign.github.io).

We will continue to update the content of this repo and recommend that you pull regularly, ensuring access to the most up to date version of the project, including addended code examples, updates for future Unity versions, more robust scenes, and new audio files for the examples.

If you find an error in the book or a bug in the project, please [open an issue](https://github.com/PrinciplesGameAudioAndSoundDesign/PGASD-2ndEdition/issues) and include the chapter, section, and page number. For project bugs, also include your Unity version and operating system. When pointing out an issue with a programming example, please include the page number, code block number in that page, and line number.


## Project Info

Built with Unity 6000.6.10f1. This may be updated in the future.

> **Note:** The project uses both the legacy Input Manager and the Input System package, so *Active Input Handling* is set to **Both**. If an example doesn't respond to input, check **Edit > Project Settings... > Player > Other Settings > Active Input Handling**.


## Installation

### 1. Install the requirements

- **[Git](https://git-scm.com/downloads)**
- **[Git LFS](https://git-lfs.com)** (Large File Storage). This repository stores its audio files and other large assets with Git LFS. Without it, those files download as small placeholder files and the examples will play no sound.
    - **macOS:** `brew install git-lfs`
    - **Windows:** Git LFS is included with [Git for Windows](https://git-scm.com/downloads/win).
    - **Linux:** `sudo apt install git-lfs` (or your distribution's package manager)
- **[Unity Hub](https://unity.com/download)** and Unity **6000.6.10f1**. If you don't have this version yet, Unity Hub will offer to install it when you open the project.

After installing Git LFS, run this once to set it up on your computer:

```bash
git lfs install
```

### 2. Clone the repository

In your terminal, go to the folder where you'd like to keep the project and run:

```bash
git clone https://github.com/PrinciplesGameAudioAndSoundDesign/PGASD-2ndEdition.git
```

> **Already cloned without Git LFS?** Run `git lfs install`, then run `git lfs pull` from inside the `PGASD-2ndEdition` folder to download the missing files.

### 3. Open the project in Unity

1. Open Unity Hub and go to **Projects**.
2. Click **Add > Add project from disk**.
3. Select the `PGASD_2nEd` folder inside the repository. Make sure you select this folder and not the repository's top-level folder.
4. Click the project to open it. The first launch takes a few minutes while Unity imports the assets.

### 4. Open an example

In the **Project** window, go to `Assets/Scenes`, open a chapter folder, and double-click a scene. Press **Play** to run it.

### Updating the project

To get the latest version, run the following from inside the repository folder:

```bash
git pull
```

If you edit the included scenes or scripts, your changes may conflict with future updates. To experiment safely, save your own copies (**File > Save As**) in a separate folder, as with the `Assets/MyAudio` folder suggested below.


## Navigation
 
The Unity project lives in the `PGASD_2nEd` folder, and all of its content is under `PGASD_2nEd/Assets`. The folders you will use most are:
 
- **Audio** holds all of the audio files used in the project. You are unlikely to need to work with these assets directly. To try the examples with your own sounds, we recommend creating a separate folder (for example, `Assets/MyAudio`) and keeping your custom assets there, so the original files stay intact.

- **Scenes** and **Scripts** are organized by chapter (`Chapter04`, `Chapter07`, and so on). Each scene demonstrates either a sound design or an audio implementation example from the book, and the scripts it uses are in the matching chapter folder under `Scripts`.

As addenda are added to the project, they will go in a separate `Addenda` folder inside both the `Scenes` and `Scripts` folders.


## List of Examples
 
Each example below is a scene in the matching chapter folder under `PGASD_2nEd/Assets/Scenes`.
 
### Chapter 4: Practical Sound Design

- **Spaceship**, Section 4.4.1 — Interactive spaceship prototype with an input-driven engine loop and thruster one-shots.
    - Use WASD to move the spaceship and hear the sound vary based on the spaceship's speed.

- **Weather Engine**, Section 4.4.3 — Adaptive weather engine that blends four weather loops with an XY pad.
    - Use your mouse to interact with the XY pad and trigger changes in the loudness of the playing AudioSources.
    - You may adapt this to be a crowd engine, and may want to try replacing the sounds assigned to the `AudioSource - Top Left`, `AudioSource - Top Right`, `AudioSource - Bottom Left`, and `AudioSource - Bottom Right` with the audio clips included in the *Audio/Weather* and *Audio/Crowd* folders.


### Chapter 6: The Audio Engine and Spatial Audio

- **Audio Random Container**, Section 6.1.4 — Demo of the Audio Random Container asset.


### Chapter 7: Coding Basics for Game Audio

> *Note: The initial `ProgrammingPlayground` examples are meant to be followed along as you read rather than run as standalone demos. For that reason, examples in sections 7.1 to 7.4 have been omitted from the repository.*

- **Ambience**, Section 7.5.1 — Selects an ambience clip based on the time of day (0–23).
    - In the editor, select the *Tester* GameObject, and change the *Time Of Day* number through the Inspector. Depending on the number you choose, you'll hear different AudioClips when you press Play.

- **Microphone Recorder**, Section 7.6.2 — Recorder storing microphone recordings in a list.
    - You may use the following keys to control the recorder:
        - <kbd>R</kbd> — Make a new recording
        - <kbd>S</kbd> — Stop recording or playback
        - <kbd>P</kbd> — Play selected sound when no recording is taking place
        - <kbd>D</kbd> — Delete selected sound when there is no playback
        - <kbd>Up</kbd> and <kbd>Down</kbd> Arrows — Select recording
        - <kbd>I</kbd> — Print microphone info to the Unity Console
        - <kbd>L</kbd> — Print length of the selected clip to the Unity Console
    - The maximum recording length for this example is 600 seconds (10 minutes).
    - The `MicrophoneRecorder` script is in the *Tester* GameObject. You can adjust the recording duration through the Inspector.
    - Note that all recordings created with this script are lost when you exit Play mode. However, it is possible to adapt this script to save your recordings to a specified location within your project.

- **MIDI Keyboard**, Section 7.6.3 — Keyboard trigger system emulating a traditional MIDI keyboard.
    - You may use your computer keyboard to trigger samples. The layout follows the one-octave keyboard layout common in DAWs: <kbd>A</kbd>, <kbd>W</kbd>, <kbd>S</kbd>, <kbd>E</kbd>, <kbd>D</kbd>, <kbd>F</kbd>, <kbd>T</kbd>, <kbd>G</kbd>, <kbd>Y</kbd>, <kbd>H</kbd>, <kbd>U</kbd>, <kbd>J</kbd>
    - The `MidiKeyboard` script is in the *Tester* GameObject. Here, you can change the samples and select whether your keyboard should behave monophonically or polyphonically.
    - Note that this example doesn't send or receive MIDI; it only replicates a MIDI keyboard's layout for playback.

- **Alarm**, Section 7.7 — Coroutine-driven alarm.
    - Use number <kbd>9</kbd> on your keyboard to start the alarm and number <kbd>0</kbd> to stop it.
    - The `Alarm` script is in the *Tester* GameObject.

- **Fades**, Section 7.7.1 — Fade-in / fade-out demonstration.
    - When you press Play, you will hear playback gradually fade-in.
    - When you are ready, use the <kbd>F</kbd> key to launch a fade-out.
    - Since the script is guarding the coroutine with a Boolean, you'll need to wait until the whole fade-in has completed before you can start a fade-out. If you want immediate control of your fades, you may want to remove the `_inCoroutine` Boolean altogether.
    - The `Fades` script is in the *Tester* GameObject. You can adjust fade duration through this component.


### Chapter 8: Basics of OOP and Defining Interactivity
 
- **Sonic Pellets**, Section 8.2.3 — Spawns sonic pellet prefabs at runtime, shifting their pitch in relation to the spawning distance from the player.
    - Press <kbd>Space</kbd> to spawn sonic pellets

- **Raycast Navigation**, Section 8.3.5 — An object that uses raycasting for spatial navigation avoiding walls.

- **Audio Manager**, Section 8.3.6 — A singleton audio manager that other objects call for playback.
    - You can test the audio manager behavior through the keyboard:
        - <kbd>M</kbd> — Toggle music playback
        - <kbd>A</kbd> — Play hit sound effect
        - <kbd>S</kbd> — Play jump sound effect
        - <kbd>D</kbd> — Play pickup sound effect
    - You can change the audio clips used for playback through the Inspector of the *AudioManager* GameObject.


### Chapter 9: Implementing Audio—Common Scenarios
 
- **Ambiences**, Section 9.2 — Randomized intermittent emitters.

- **Concatenation**, Section 9.3 — This scene features two examples: sample concatenation and sample-accurate scheduling with `PlayScheduled()`.
    - The first example uses the `ConcatenatedPlayback` script, which demonstrates concatenation of two sounds. You may trigger the sounds using the <kbd>1</kbd> key, and can modify the sounds being concatenated through the Inspector of the *ConcatenatedPlayback* GameObject.
    - The second example uses the `ScheduledConcatenation` script, which demonstrates concatenation of all the sounds in an array. You may trigger the sounds using the <kbd>2</kbd> key and modify the samples in the array through the *ScheduledConcatenation* GameObject.

- **Bouncing Ball**, Section 9.4.1 — Plays a sound when an object collides with the ground.

- **Projectiles**, Section 9.4.2 — A turret that fires projectiles with velocity-based sample selection on impact.
    - Use *left-click* to fire projectiles.
    - Note that you might hear a second sound if the projectile reaches the ground.

- **Smart Audio Source**, Section 9.5.1 — Occlusion simulated by raycasting from the source to the listener.

- **Layer-Masked Raycasting**, Section 9.5.2 — Occlusion limited to an obstacles layer, avoiding the pebble effect.
    - Note that for this example, only the orange obstacle will occlude the sound.
    - This example has also been updated from the example in the book to simplify the `LayerMask` selection by serializing a `LayerMask` variable, allowing you to work with a dropdown menu for multiple mask selection directly from the Inspector. See the [Errata](https://principlesgameaudioandsounddesign.github.io/errata.html) page for more information.

- **Distance-Based Playback**, Section 9.5.3 — Stops a 3D AudioSource beyond its falloff radius and resumes where it left off.

- **Virtual Playhead**, Section 9.5.4 — Sample-accurate example of distance-based playback, keeping track of playback position using `AudioSettings.dspTime`.

- **Animation Events**, Section 9.6 — Triggers randomized sounds from animation events.
    - To make the effect easy to hear, the footsteps were replaced with drum hits.

- **Distance Crossfade**, Section 9.7 — Crossfades between a near and a far source based on distance to the listener.
    - Note that the GameObjects that hold the AudioSources for the near and far sounds are children of the *Sphere* GameObject, allowing their position to change as the parent moves.


## License

Unless otherwise noted, the contents of this repository are released under the [MIT License](LICENSE). Third-party assets are covered by their own licenses.

The companion Unity project uses the following third-party assets:

- **[Starter Assets – Third Person Character Controller](https://assetstore.unity.com/packages/3d/characters/first-person-third-person-character-controllers-196526)** by Unity Technologies, used under the [Unity Companion License](https://unity.com/legal/licenses/unity-companion-license). We modified the package so that manual and custom animation events can be added to the model's animation clips. The original license file is kept in `Assets/StarterAssets/`.

- **[3D Sci-Fi Spaceship Hurricane 07F](https://assetstore.unity.com/packages/3d/vehicles/3d-sci-fi-spaceship-hurricane-07f-292460)** by Entian-Astea, licensed under the [Standard Unity Asset Store EULA](https://unity.com/legal/as-terms).
