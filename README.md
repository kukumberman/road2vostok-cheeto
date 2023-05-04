# road2vostok-cheeto

## Description

Goal of this project was to implement runtime usage of [ImGui](https://github.com/psydack/uimgui) in [BepInEx](https://github.com/BepInEx/BepInEx) plugin for games made in **Unity Engine**.

Also demonstrated example of using **URP** `ScriptableRendererFeature` in runtime (heavy relies on `System.Reflection` due to inaccessible official runtime API).

## Installation

Here is instruction list in case if someone finds it useful, or for myself in case if I will return to the project in the future, or I will use it in other projects.

- Install **BepInEx**
- Run game once to create file structure required for using pluings
- Create empty project in **Unity**
- Install **UImGui**
- Create asset-bundle (named `mybundle`) with selected shaders and put into game folder ([ImguiInstaller.cs#L30](https://github.com/kukumberman/road2vostok-cheeto/blob/main/Plugin/Cheeto/Core/ImguiInstaller.cs#L30))
- Build emtpy project and navigate to its path
- Copy all required `.dll` files to coresponding game folder
  - Managed:
    - `UImGui.dll`
    - `ImGui.NET.dll`
    - `netstandard.dll`
    - `System.Runtime.CompilerServices.Unsafe.dll`
  - Plugins:
    - `cimgui.dll`
    - all others
- Add attribute `[assembly: InternalsVisibleTo("<AssemblyName>")]` to `UImGui.dll` to be able to use internal classes in runtime (replace `<AssemblyName>` with your name) (for example use [dnSpy](https://github.com/dnSpy/dnSpy)), other possible option is to remove redurant `internal` keyword from package and skip this step
- Create your own plugin and use **ImGui** functionality in runtime
- Congratulations :partying_face:

## Video preview

<a href="./preview.mp4">Link</a>
