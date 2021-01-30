# KRG #

__KRG__ a Unity 2019 framework library

KRG serves as a launchpad for initializing any Unity project and providing core functionality for a variety of purposes.
KRG also provides support for many third-party libraries, such as TextMesh Pro.

## NEW PREREQUISITE ##

You will need this: https://github.com/k76-0G/0G

## One-Time Setup ##

1. With Unity closed, place the KRG folder inside your game's Assets folder.
2. Open Unity, and from the KRG menu, select "Create KRGLoader".
3. Then, from the KRG menu, select "Create KRGConfig". 

## General Implementation ##

* Every scene needs to include an instance of your game's KRGLoader prefab. That's it.

## Optional Unity Install ##

* NOTE: Now that Unity Hub has been released, new paths are needed. Here are the classic instructions:
* Copy KRG\UnityInstall\81-C# Script-NewBehaviourScript.cs.txt to...
  * macOS: /Applications/Unity/Hub/Editor/{CurrentVersion}/Unity.app/Contents/Resources/ScriptTemplates
  * Windows: C:\Program Files\Unity\Editor\Data\Resources\ScriptTemplates
* This will need to be redone if installing a new version of Unity.

## Coding With "G" ##

      [[ ]]
    [[  G  ]]
      [[ ]]

G is your God, and **you** control God.

Example Usage:

    using KRG;
    //...
    G.audio.Volume++;

* The G singleton MonoBehaviour provides a narration, or an order, to the chaos of the equal execution of all possible objects.
* KRG.G's script execution order is -30000 (to be run before everything except Rewired).

## Tips (To Be Detailed Later) ##

* KRG_G_CUSTOM

## Implementing Abstract Classes ##

Many of the KRG classes derived from MonoBehaviour and ScriptableObject are abstract. Simply create a derived class in your game project for each of these. By having and using your own derived classes from the start, you can add and modify functionality at any time without having to replace components on your GameObjects/prefabs if you decide to do so at a later time.

## Enum Replacement ##

Enums used in KRG can be swapped with your own custom enums when they use EnumAttribute. Let's take the following example:

    [Enum(typeof(SomeEnum))]
    private int m_SomeEnumValue;

You can change KRG.SomeEnum to MyGame.SomeEnum simply by creating an EnumDrawer.cs file in an Editor folder and using the following:

    namespace MyGame
    {
        [CustomPropertyDrawer(typeof(EnumAttribute))]
        public class EnumDrawer : KRG.EnumDrawer
        {
            protected override bool SwapEnum(ref string stringType)
            {
                stringType = stringType.Replace("KRG.", "MyGame.");
                return true;
            }
        }
    }

Furthermore, you can customize this method to make "SomeEnum" become any enum you want!

## Game Object Body System Overview ##

(To be added.)

## Attack System Overview ##

(To be added.)

## Damage System Overview ##

(To be revised.)

Do the following for any object you want to be damaged:

1. Create Damage Profile asset, and tweak values.
2. On object root, add DamageTaker component, and assign Damage Profile.
3. On object sub-GameObject, set hit box Layer, add HitBox component, and assign DamageTaker.

## Third-Party Library Support ##

KRG has some functionality based on certain third-party libraries that can be obtained from the Asset Store. In order to enable this functionality, you must import a package into your project, and then add a specific define symbol to your player settings based on the library that was added. As follows is the list of define symbols for the currently supported libraries:

* `KRG_X_ODIN` - Namespace: Sirenix.OdinInspector
* `NS_DG_TWEENING` - Namespace: DG.Tweening (DOTween)
* `NS_FMOD` - Namespace: FMOD & FMODUnity
* `NS_TMPRO` - Namespace: TMPro (TextMesh Pro)
* `NS_UGIF` - Namespace: uGIF

Rewired (by Guavaman Enterprises) is also supported, but currently uses no define symbol.

### TextMesh Pro (Additional Info) ###

* TextMesh Pro resources folder should be "Text/".
* If using the old, paid (source code) version of TextMesh Pro, you will need the `NS_TMPRO_PAID` define symbol (compiler flag) *in addition* to `NS_TMPRO`.

## Naming & Organizational Conventions ##

For the most part, established Unity and Microsoft C# conventions are used. Some exceptions and explicit definitions follow:

* Private fields: private object m_SomePrivateField;
* Properties: public object SomeProperty => m_SomePrivateField;

## Documentation Standards ##

Documentation thus far has been sporadic, but going forward, every class should have a comment summary explaining the following four things:

1. What the class does (what its purpose is).
2. How to use the class (e.g. what methods or functions to call).
3. Which related classes or objects are required, if any (e.g. prefabs and scripts).
4. How this class can be extended and customized, if possible (typically it should be).

## Disclaimer ##

I do not endorse any religion or deity. The term "God" is used purely as an apt programming metaphor.
