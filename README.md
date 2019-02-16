# Pepperoni

This API is the first serious effort towards enabling Yo! Noid 2 to develop mods while leaving (most) of the game assets intact.
Most of the assembly patching work is greatly simplified thanks to MonoMod. Check it out here: https://github.com/0x0ade/MonoMod
Also many thanks to HollowKnight Modding Community, whose framework has been leveraged greatly to form the foundation for Pepperoni:
https://github.com/seanpr96/HollowKnight.Modding

## Building the API for Windows

1. Clone this repo.
2. Go to `%NoidGameInstallPath%/noid_Data/Managed/` and copy it's contents to the `Vanilla` folder in this repository. (Create the Vanilla folder if it does not exist under Pepperoni subfolder. This is where MonoMod folder resides)
3. Open the solution in Visual Studio 2017 (May work in other versions, only tested on VS2017 Community Edition)
4. Set the build configuration to Debug.
5. The patched assembly should be in `RepoPath/bin/Debug/OUTPUT_Assembly-CSharp.dll` 
6. Copy & Rename OUTPUT_Assembly-CSharp.dll to `%NoidGameInstallPath%/noid_data/Managed/Assembly-CSharp.dll`. Replace the existing library if asked.
7. Create a folder named "Mods" under `%NoidGameInstallPath%/noid_data/Managed` and place mod dlls here.

### Running Mods

See step 7 above.


## Authors

SpectralPlatypus

Also many thanks to YN2 Speedrunning community for testing the Mod API in its early stages!
