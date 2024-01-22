# Setting up the Project
## Download Unity
### Download/Install Unity Hub
https://unity3d.com/get-unity/download
### Install Unity
* Open Unity Hub
* Go the [Unity Download Archive](https://unity3d.com/get-unity/download/archive) and install version *2023.2.6f1* via the Unity Hub link
## Download this Project
You can download and install the project either via Github Desktop or by using the git CLI (command-line interface) -- most folks should opt to use Github Desktop unless you're already comfortable with - or want to learn how to use - the CLI
### Using Github Desktop
* If you don't know git very well, the best way to clone this project is to use https://desktop.github.com/ -- the website will have instructions for setting up github and cloning this repository.
* To open the project, you will need to have the [git CLI installed](https://git-scm.com/downloads)
### Using git CLI
* You may need to manually initialize git LFS. Once cloned use `git lfs install` and `git lfs pull` to pull any binary files
## Open the Project in Unity
From the Unity Hub | Projects tab, click "Open," navigate to where Git checked out the project, and select the SSJ23-Idle directory

#
# Project Best Practices
### DO Namespace your code with `namespace LeftOut.GameJam`
This will help us find each other's code in our IDE's and will make refactoring later easier.
### DO Work in your own branch
Name your branch with a prefix unique to you, and the word `staging`. Devin's branch will be called `devin/staging`, for example. If you are an old git pro and want to do more granular branch management, that's fine, but try to merge your other forks back into your personal `staging` branch before merging into `main`
### DO Write useful commit messages
It's tempting to write something like "changed stuff" or "small fixes" as your commit message, but please don't! Useful commit messages are the best way we have of keeping track of what everyone is doing. Use short imperative clauses to describe what the commit will do. "Add trees to main Scene," "Add support for using xbox controller; Fix issues with background music," etc.
### DO Create a new Unity scene to test your feature
Create a scene for yourself and do new work there. Only include the bare minimum objects and components in your scene to validate what you're working on. If you're building/testing a lot of things, consider making your own directory to store your Scenes.
### DO Use ScriptableObjects for exposing global state and raising events (and avoid Singletons)
Unity youtubers will often encourage the use of the Singleton pattern for holding all the state in a scene. This is usually not a great approach! Review [this blog post](https://unity.com/how-to/architect-game-code-scriptable-objects) or [this quick tutorial](https://www.youtube.com/watch?v=WLDgtRNK2VE) to observe how - if your code doesn't need a `Transform` or any `Update()` calls - you can put it into a ScriptableObject that can be referenced by anything that needs access to that information.
### DO NOT Modify shared resources unless you are integrating features.
... and check with people first before merging changes back in. Prototype in your own scenes, don't apply prefab overrides until they're ready to be merged into main. If you're working extensively on shared prefabs across multiple scenes, consider working with a Prefab Variant until you're ready to replace the original.
### DO Regular merge commit from staging to main 
When your code is ready to go back into our main branch, first merge `main` into your `staging` branch, deal with any conflict, then merge the result back to main. If you have other branches, merge those into your personal `staging` branch first.
### DO Respect the separation between Runtime and Editor code
If it says `using UnityEditor` at the top, it should either be in the Editor folder, or you need to wrap it with `#if UNITY_EDITOR` to keep it from breaking the build.
### DO Check for permissively licensed Open Source or free Asset Store projects before starting a new system
Always double-check that you aren't about to write a bunch of code that already exists in a useable format. But, be wary of free Script assets... most of the time they're more of a pain to integrate than they're worth!
## DO Always ask for help when you need it
The point of the Game Jam is to Learn New Things! Check the Resources channel for previously shared resources or ask for help/feedback in the support channel!
