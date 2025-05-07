using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TrueBRChaos.Patches
{
    internal class EventPatch_InputHandler : HarmonyPatch
    {
        [HarmonyPatch(typeof(UserInputHandler), "PollInputs")]
        public static class UserInputHandler_PollInputs_Patch
        {
            public static bool Prefix(ref UserInputHandler.InputBuffer inputBuffer, GameInput ___gameInput)
            {
                inputBuffer.jumpButtonNew               = InputsOverride.JumpNew.Item1      ? InputsOverride.JumpNew.Item2      : ___gameInput.GetButtonNew             (Inputs.JumpNew,    0);
                inputBuffer.jumpButtonHeld              = InputsOverride.JumpHeld.Item1     ? InputsOverride.JumpHeld.Item2     : ___gameInput.GetButtonHeld            (Inputs.JumpHeld,   0);
                inputBuffer.sprayButtonHeldPrevFrame    = InputsOverride.SprayPrev.Item1    ? InputsOverride.SprayPrev.Item2    : ___gameInput.GetButtonHeldPrevFrame   (Inputs.SprayPrev,  0);
                inputBuffer.sprayButtonHeld             = InputsOverride.SprayHeld.Item1    ? InputsOverride.SprayHeld.Item2    : ___gameInput.GetButtonHeld            (Inputs.SprayHeld,  0);
                inputBuffer.sprayButtonNew              = InputsOverride.SprayNew.Item1     ? InputsOverride.SprayNew.Item2     : ___gameInput.GetButtonNew             (Inputs.SprayNew,   0);
                inputBuffer.boostButtonHeld             = InputsOverride.BoostHeld.Item1    ? InputsOverride.BoostHeld.Item2    : ___gameInput.GetButtonHeld            (Inputs.BoostHeld,  0);
                inputBuffer.boostButtonNew              = InputsOverride.BoostNew.Item1     ? InputsOverride.BoostNew.Item2     : ___gameInput.GetButtonNew             (Inputs.BoostNew,   0);
                inputBuffer.trick1ButtonHeld            = InputsOverride.Trick1Held.Item1   ? InputsOverride.Trick1Held.Item2   : ___gameInput.GetButtonHeld            (Inputs.Trick1Held, 0);
                inputBuffer.trick1ButtonNew             = InputsOverride.Trick1New.Item1    ? InputsOverride.Trick1New.Item2    : ___gameInput.GetButtonNew             (Inputs.Trick1New,  0);
                inputBuffer.trick2ButtonHeld            = InputsOverride.Trick2Held.Item1   ? InputsOverride.Trick2Held.Item2   : ___gameInput.GetButtonHeld            (Inputs.Trick2Held, 0);
                inputBuffer.trick2ButtonNew             = InputsOverride.Trick2New.Item1    ? InputsOverride.Trick2New.Item2    : ___gameInput.GetButtonNew             (Inputs.Trick2New,  0);
                inputBuffer.trick3ButtonHeld            = InputsOverride.Trick3Held.Item1   ? InputsOverride.Trick3Held.Item2   : ___gameInput.GetButtonHeld            (Inputs.Trick3Held, 0);
                inputBuffer.trick3ButtonNew             = InputsOverride.Trick3New.Item1    ? InputsOverride.Trick3New.Item2    : ___gameInput.GetButtonNew             (Inputs.Trick3New,  0);
                inputBuffer.switchStyleButtonHeld       = InputsOverride.SwitchHeld.Item1   ? InputsOverride.SwitchHeld.Item2   : ___gameInput.GetButtonHeld            (Inputs.SwitchHeld, 0);
                inputBuffer.switchStyleButtonNew        = InputsOverride.SwitchNew.Item1    ? InputsOverride.SwitchNew.Item2    : ___gameInput.GetButtonNew             (Inputs.SwitchNew,  0);
                inputBuffer.slideButtonHeld             = InputsOverride.SlideHeld.Item1    ? InputsOverride.SlideHeld.Item2    : ___gameInput.GetButtonHeld            (Inputs.SlideHeld,  0);
                inputBuffer.slideButtonNew              = InputsOverride.SlideNew.Item1     ? InputsOverride.SlideNew.Item2     : ___gameInput.GetButtonNew             (Inputs.SlideNew,   0);
                inputBuffer.danceButtonHeld             = InputsOverride.DanceHeld.Item1    ? InputsOverride.DanceHeld.Item2    : ___gameInput.GetButtonHeld            (Inputs.DanceHeld,  0);
                inputBuffer.danceButtonNew              = InputsOverride.DanceNew.Item1     ? InputsOverride.DanceNew.Item2     : ___gameInput.GetButtonNew             (Inputs.DanceNew,   0);
                inputBuffer.walkButtonHeld              = InputsOverride.WalkHeld.Item1     ? InputsOverride.WalkHeld.Item2     : ___gameInput.GetButtonHeld            (Inputs.WalkHeld,   0);
                inputBuffer.walkButtonNew               = InputsOverride.WalkNew.Item1      ? InputsOverride.WalkNew.Item2      : ___gameInput.GetButtonNew             (Inputs.WalkNew,    0);

                inputBuffer.moveAxisX                   = InputsOverride.AxisX.Item1 ? InputsOverride.AxisX.Item2 : ___gameInput.GetAxis(Inputs.AxisX, 0);
                inputBuffer.moveAxisY                   = InputsOverride.AxisX.Item1 ? InputsOverride.AxisY.Item2 : ___gameInput.GetAxis(Inputs.AxisY, 0);

                return false;
            }
        }

        public static void OverrideAxis(Axis axis, float value, bool overrided = true)
        {
            switch(axis)
            {
                case Axis.AxisX:
                    InputsOverride.AxisX.Item1 = overrided;
                    InputsOverride.AxisX.Item2 = Mathf.Clamp(value, -1f, 1f);
                break;

                case Axis.AxisY:
                    InputsOverride.AxisY.Item1 = overrided;
                    InputsOverride.AxisY.Item2 = Mathf.Clamp(value, -1f, 1f);
                break;
            }
        }

        public static void OverrideInput(InputEvents input, bool shouldOverride, bool value = false)
        {
            (bool, bool) inputState = (shouldOverride, value);
            switch (input)
            {
                case InputEvents.JumpNew:   InputsOverride.JumpNew      = inputState; break;
                case InputEvents.JumpHeld:  InputsOverride.JumpHeld     = inputState; break;

                case InputEvents.SprayPrev: InputsOverride.SprayPrev    = inputState; break;
                case InputEvents.SprayHeld: InputsOverride.SprayHeld    = inputState; break;
                case InputEvents.SprayNew:  InputsOverride.SprayNew     = inputState; break;

                case InputEvents.BoostHeld: InputsOverride.BoostHeld    = inputState; break;
                case InputEvents.BoostNew:  InputsOverride.BoostNew     = inputState; break;

                case InputEvents.Trick1Held:InputsOverride.Trick1Held   = inputState; break;
                case InputEvents.Trick1New: InputsOverride.Trick1Held   = inputState; break;

                case InputEvents.Trick2Held:InputsOverride.Trick2Held   = inputState; break;
                case InputEvents.Trick2New: InputsOverride.Trick2Held   = inputState; break;

                case InputEvents.Trick3Held:InputsOverride.Trick3Held   = inputState; break;
                case InputEvents.Trick3New: InputsOverride.Trick3Held   = inputState; break;

                case InputEvents.SwitchHeld:InputsOverride.SwitchHeld   = inputState; break;
                case InputEvents.SwitchNew: InputsOverride.SwitchNew    = inputState; break;

                case InputEvents.SlideHeld: InputsOverride.SlideHeld    = inputState; break;
                case InputEvents.SlideNew:  InputsOverride.SlideNew     = inputState; break;

                case InputEvents.DanceHeld: InputsOverride.DanceHeld    = inputState; break;
                case InputEvents.DanceNew:  InputsOverride.DanceNew     = inputState; break;

                case InputEvents.WalkHeld:  InputsOverride.WalkHeld     = inputState; break;
                case InputEvents.WalkNew:   InputsOverride.WalkNew      = inputState; break;
            }
        }

        public enum Axis
        {
            AxisX,
            AxisY
        }

        public static void SetInput(params Input[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                switch (inputs[i].inputEvent)
                {
                    case InputEvents.JumpNew:   Inputs.JumpNew      = inputs[i].newInput; break;
                    case InputEvents.JumpHeld:  Inputs.JumpHeld     = inputs[i].newInput; break;

                    case InputEvents.SprayPrev: Inputs.SprayPrev    = inputs[i].newInput; break;
                    case InputEvents.SprayHeld: Inputs.SprayHeld    = inputs[i].newInput; break;
                    case InputEvents.SprayNew:  Inputs.SprayNew     = inputs[i].newInput; break;

                    case InputEvents.BoostHeld: Inputs.BoostHeld    = inputs[i].newInput; break;
                    case InputEvents.BoostNew:  Inputs.BoostNew     = inputs[i].newInput; break;

                    case InputEvents.Trick1Held:Inputs.Trick1Held   = inputs[i].newInput; break;
                    case InputEvents.Trick1New: Inputs.Trick1Held   = inputs[i].newInput; break;

                    case InputEvents.Trick2Held:Inputs.Trick2Held   = inputs[i].newInput; break;
                    case InputEvents.Trick2New: Inputs.Trick2Held   = inputs[i].newInput; break;

                    case InputEvents.Trick3Held:Inputs.Trick3Held   = inputs[i].newInput; break;
                    case InputEvents.Trick3New: Inputs.Trick3Held   = inputs[i].newInput; break;

                    case InputEvents.SwitchHeld:Inputs.SwitchHeld   = inputs[i].newInput; break;
                    case InputEvents.SwitchNew: Inputs.SwitchNew    = inputs[i].newInput; break;

                    case InputEvents.SlideHeld: Inputs.SlideHeld    = inputs[i].newInput; break;
                    case InputEvents.SlideNew:  Inputs.SlideNew     = inputs[i].newInput; break;

                    case InputEvents.DanceHeld: Inputs.DanceHeld    = inputs[i].newInput; break;
                    case InputEvents.DanceNew:  Inputs.DanceNew     = inputs[i].newInput; break;

                    case InputEvents.WalkHeld:  Inputs.WalkHeld     = inputs[i].newInput; break;
                    case InputEvents.WalkNew:   Inputs.WalkNew      = inputs[i].newInput; break;

                    case InputEvents.AxisX:     Inputs.AxisX        = inputs[i].newInput; break;
                    case InputEvents.AxisY:     Inputs.AxisY        = inputs[i].newInput; break;
                }
            }
        }

        public struct Input
        {
            public InputEvents  inputEvent;
            public int          newInput;

            public Input(InputEvents inputEvent, InputIDs newInput)
            {
                this.inputEvent = inputEvent;
                this.newInput   = (int)newInput;
            }
        }

        public static void RestoreInput(params InputEvents[] inputEvents)
        {
            List<Input> inputsToRestore = new List<Input>();

            for (int i = 0; i < inputEvents.Length; i++)
            {
                switch (inputEvents[i])
                {
                    case InputEvents.JumpNew:   inputsToRestore.Add(new Input(InputEvents.JumpNew,       InputIDs.Jump));     break;
                    case InputEvents.JumpHeld:  inputsToRestore.Add(new Input(InputEvents.JumpHeld,      InputIDs.Jump));     break;

                    case InputEvents.SprayPrev: inputsToRestore.Add(new Input(InputEvents.SprayPrev,     InputIDs.Spray));    break;
                    case InputEvents.SprayHeld: inputsToRestore.Add(new Input(InputEvents.SprayHeld,     InputIDs.Spray));    break;
                    case InputEvents.SprayNew:  inputsToRestore.Add(new Input(InputEvents.SprayNew,      InputIDs.Spray));    break;

                    case InputEvents.BoostHeld: inputsToRestore.Add(new Input(InputEvents.BoostHeld,     InputIDs.Boost));    break;
                    case InputEvents.BoostNew:  inputsToRestore.Add(new Input(InputEvents.BoostNew,      InputIDs.Boost));    break;

                    case InputEvents.Trick1Held:inputsToRestore.Add(new Input(InputEvents.Trick1Held,    InputIDs.Trick1));   break;
                    case InputEvents.Trick1New: inputsToRestore.Add(new Input(InputEvents.Trick1New,     InputIDs.Trick1));   break;

                    case InputEvents.Trick2Held:inputsToRestore.Add(new Input(InputEvents.Trick2Held,    InputIDs.Trick2));   break;
                    case InputEvents.Trick2New: inputsToRestore.Add(new Input(InputEvents.Trick2New,     InputIDs.Trick2));   break;

                    case InputEvents.Trick3Held:inputsToRestore.Add(new Input(InputEvents.Trick3Held,    InputIDs.Trick3));   break;
                    case InputEvents.Trick3New: inputsToRestore.Add(new Input(InputEvents.Trick3New,     InputIDs.Trick3));   break;

                    case InputEvents.SwitchHeld:inputsToRestore.Add(new Input(InputEvents.SwitchHeld,    InputIDs.Switch));   break;
                    case InputEvents.SwitchNew: inputsToRestore.Add(new Input(InputEvents.SwitchNew,     InputIDs.Switch));   break;

                    case InputEvents.SlideHeld: inputsToRestore.Add(new Input(InputEvents.SlideHeld,     InputIDs.Slide));    break;
                    case InputEvents.SlideNew:  inputsToRestore.Add(new Input(InputEvents.SlideNew,      InputIDs.Slide));    break;

                    case InputEvents.DanceHeld: inputsToRestore.Add(new Input(InputEvents.DanceHeld,     InputIDs.Dance));    break;
                    case InputEvents.DanceNew:  inputsToRestore.Add(new Input(InputEvents.DanceNew,      InputIDs.Dance));    break;

                    case InputEvents.WalkHeld:  inputsToRestore.Add(new Input(InputEvents.WalkHeld,      InputIDs.Walk));     break;
                    case InputEvents.WalkNew:   inputsToRestore.Add(new Input(InputEvents.WalkNew,       InputIDs.Walk));     break;

                    case InputEvents.AxisX:     inputsToRestore.Add(new Input(InputEvents.AxisX,         InputIDs.AxisX));    break;
                    case InputEvents.AxisY:     inputsToRestore.Add(new Input(InputEvents.AxisY,         InputIDs.AxisY));    break;
                }
            }

            if (inputsToRestore.Count > 0)
                SetInput(inputsToRestore.ToArray());
        }

        public static void RestoreInputs()
        {
            RestoreInput(InputEvents.JumpNew);
            RestoreInput(InputEvents.JumpHeld);

            RestoreInput(InputEvents.SprayPrev);
            RestoreInput(InputEvents.SprayHeld);
            RestoreInput(InputEvents.SprayNew);

            RestoreInput(InputEvents.BoostHeld);
            RestoreInput(InputEvents.BoostNew);

            RestoreInput(InputEvents.Trick1Held);
            RestoreInput(InputEvents.Trick1New);

            RestoreInput(InputEvents.Trick2Held);
            RestoreInput(InputEvents.Trick2New);

            RestoreInput(InputEvents.Trick3Held);
            RestoreInput(InputEvents.Trick3New);

            RestoreInput(InputEvents.SwitchHeld);
            RestoreInput(InputEvents.SwitchNew);

            RestoreInput(InputEvents.SlideHeld);
            RestoreInput(InputEvents.SlideNew);

            RestoreInput(InputEvents.DanceHeld);
            RestoreInput(InputEvents.DanceNew);

            RestoreInput(InputEvents.WalkHeld);
            RestoreInput(InputEvents.WalkNew);

            RestoreInput(InputEvents.AxisX);
            RestoreInput(InputEvents.AxisY);
        }

        public static void SetInput(ref int input, int newInput)
        {
            input = newInput;
        }

        public class Inputs
        {
            public static int JumpNew       = (int)InputIDs.Jump;
            public static int JumpHeld      = (int)InputIDs.Jump;

            public static int SprayPrev     = (int)InputIDs.Spray;
            public static int SprayHeld     = (int)InputIDs.Spray;
            public static int SprayNew      = (int)InputIDs.Spray;

            public static int BoostHeld     = (int)InputIDs.Boost;
            public static int BoostNew      = (int)InputIDs.Boost;

            public static int Trick1Held    = (int)InputIDs.Trick1;
            public static int Trick1New     = (int)InputIDs.Trick1;

            public static int Trick2Held    = (int)InputIDs.Trick2;
            public static int Trick2New     = (int)InputIDs.Trick2;

            public static int Trick3Held    = (int)InputIDs.Trick3;
            public static int Trick3New     = (int)InputIDs.Trick3;

            public static int SwitchHeld    = (int)InputIDs.Switch;
            public static int SwitchNew     = (int)InputIDs.Switch;

            public static int SlideHeld     = (int)InputIDs.Slide;
            public static int SlideNew      = (int)InputIDs.Slide;

            public static int DanceHeld     = (int)InputIDs.Dance;
            public static int DanceNew      = (int)InputIDs.Dance;

            public static int WalkHeld      = (int)InputIDs.Walk;
            public static int WalkNew       = (int)InputIDs.Walk;

            public static int AxisX         = (int)InputIDs.AxisX;
            public static int AxisY         = (int)InputIDs.AxisY;
        }

        private class InputsOverride
        {
            private static readonly (bool, bool) initialState = (false, false);

            internal static (bool, bool) JumpNew    = initialState;
            internal static (bool, bool) JumpHeld   = initialState;

            internal static (bool, bool) SprayPrev  = initialState;
            internal static (bool, bool) SprayHeld  = initialState;
            internal static (bool, bool) SprayNew   = initialState;

            internal static (bool, bool) BoostHeld  = initialState;
            internal static (bool, bool) BoostNew   = initialState;

            internal static (bool, bool) Trick1Held = initialState;
            internal static (bool, bool) Trick1New  = initialState;

            internal static (bool, bool) Trick2Held = initialState;
            internal static (bool, bool) Trick2New  = initialState;

            internal static (bool, bool) Trick3Held = initialState;
            internal static (bool, bool) Trick3New  = initialState;

            internal static (bool, bool) SwitchHeld = initialState;
            internal static (bool, bool) SwitchNew  = initialState;

            internal static (bool, bool) SlideHeld  = initialState;
            internal static (bool, bool) SlideNew   = initialState;

            internal static (bool, bool) DanceHeld  = initialState;
            internal static (bool, bool) DanceNew   = initialState;

            internal static (bool, bool) WalkHeld   = initialState;
            internal static (bool, bool) WalkNew    = initialState;

            internal static (bool, float) AxisX     = (false, 0f);
            internal static (bool, float) AxisY     = (false, 0f);
        }

        public enum InputEvents
        {
            JumpNew,
            JumpHeld,

            SprayPrev,
            SprayHeld,
            SprayNew,

            BoostHeld,
            BoostNew,

            Trick1Held,
            Trick1New,

            Trick2Held,
            Trick2New,

            Trick3Held,
            Trick3New,

            SwitchHeld,
            SwitchNew,

            SlideHeld,
            SlideNew,

            DanceHeld,
            DanceNew,

            WalkHeld,
            WalkNew,

            AxisX,

            AxisY
        }

        public enum InputIDs
        {
            Jump        = 7,
            Spray       = 10,
            Boost       = 18,
            Trick1      = 15,
            Trick2      = 12,
            Trick3      = 65,
            Switch      = 11,
            Slide       = 8,
            Dance       = 17,
            Walk        = 16,
            AxisX       = 5,
            AxisY       = 6,
            Disabled    = -1
        }
    }
}
