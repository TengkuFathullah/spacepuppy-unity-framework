﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace com.spacepuppy.UserInput.UnityInput
{

    public class ButtonInputSignature : AbstractInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;

        #endregion

        #region CONSTRUCTOR

        public ButtonInputSignature(string id, string unityInputId)
            : base(id)
        {
            this.UnityInputId = unityInputId;
        }

        public ButtonInputSignature(string id, int hash, string unityInputId)
            : base(id, hash)
        {
            this.UnityInputId = unityInputId;
        }

        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId));
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId));
        }

        #endregion

    }

    /// <summary>
    /// Dual mode input signature. It allows for axes to act like a button. It will prefer button presses over it. 
    /// Very useful if you have dual inputs for the same action that should act like buttons. For instance both pressing 
    /// up on left stick, or pressing A to jump.
    /// </summary>
    public class AxleButtonInputSignature : AbstractInputSignature, IButtonInputSignature
    {

        #region Fields

        public enum AxleValueConsideration
        {
            Positive = 0,
            Negative = 1,
            Absolute = 2
        }

        private ButtonState _current;
        private ButtonState _currentFixed;

        #endregion

        #region CONSTRUCTOR

        public AxleButtonInputSignature(string id, string unityInputId, AxleValueConsideration consideration = AxleValueConsideration.Positive)
            : base(id)
        {
            this.AxisButtonDeadZone = 0.707f;
            this.UnityInputId = unityInputId;
            this.Consideration = consideration;
        }

        public AxleButtonInputSignature(string id, int hash, string unityInputId, AxleValueConsideration consideration = AxleValueConsideration.Positive)
            : base(id, hash)
        {
            this.AxisButtonDeadZone = 0.5f;
            this.UnityInputId = unityInputId;
            this.Consideration = consideration;
        }

        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
        }

        /// <summary>
        /// If input is treated as an axis, where is the dead-zone of the button.
        /// </summary>
        public float AxisButtonDeadZone
        {
            get;
            set;
        }

        /// <summary>
        /// How should we consider the value returned for the axis.
        /// </summary>
        public AxleValueConsideration Consideration
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public ButtonState CurrentState
        {
            get
            {
                if(GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            ////determine based on history
            //_current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);

            switch(this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    _current = InputUtil.GetNextButtonState(_current, Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    _current = InputUtil.GetNextButtonState(_current, Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_current = InputUtil.GetNextButtonState(_current, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _current = InputUtil.GetNextButtonState(_current, Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    break;
            }
        }

        public override void FixedUpdate()
        {
            //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);

            switch (this.Consideration)
            {
                case AxleValueConsideration.Positive:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetAxis(this.UnityInputId) >= this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Negative:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetAxis(this.UnityInputId) <= -this.AxisButtonDeadZone);
                    break;
                case AxleValueConsideration.Absolute:
                    //_currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetButton(this.UnityInputId) || Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Mathf.Abs(Input.GetAxis(this.UnityInputId)) >= this.AxisButtonDeadZone);
                    break;
            }
        }

        #endregion

    }

    public class KeyboardInputSignature : AbstractInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;

        #endregion

        #region CONSTRUCTOR

        public KeyboardInputSignature(string id, KeyCode key)
            : base(id)
        {
            this.Key = key;
        }

        public KeyboardInputSignature(string id, int hash, KeyCode key)
            : base(id, hash)
        {
            this.Key = key;
        }

        #endregion

        #region Properties

        public KeyCode Key
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            ////use classic way of determining
            //if (Input.GetKeyDown(this.Key))
            //    _current = ButtonState.Down;
            //else if (Input.GetKeyUp(this.Key))
            //    _current = ButtonState.Released;
            //else if (Input.GetKey(this.Key))
            //    _current = ButtonState.Held;
            //else
            //    _current = ButtonState.None;


            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, Input.GetKey(this.Key));
        }

        public override void FixedUpdate()
        {
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetKey(this.Key));
        }

        #endregion

    }

    public class AxleInputSignature : AbstractInputSignature, IAxleInputSignature
    {

        #region Fields

        private float _current;

        #endregion

        #region CONSTRUCTOR

        public AxleInputSignature(string id, string unityInputId)
            : base(id)
        {
            this.UnityInputId = unityInputId;
        }

        public AxleInputSignature(string id, int hash, string unityInputId)
            : base(id, hash)
        {
            this.UnityInputId = unityInputId;
        }

        #endregion

        #region Properties

        public string UnityInputId
        {
            get;
            set;
        }

        public float DeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff Cutoff
        {
            get;
            set;
        }

        public bool Invert
        {
            get;
            set;
        }

        #endregion

        #region IAxleInputSignature Interface

        public float CurrentState { get { return _current; } }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            var v = Input.GetAxis(this.UnityInputId);
            if (this.Invert) v *= -1;
            _current = InputUtil.CutoffAxis(v, this.DeadZone, this.Cutoff);
        }

        #endregion

    }

    public class DualAxleInputSignature : AbstractInputSignature, IDualAxleInputSignature
    {

        #region Fields

        private string _xAxisId;
        private string _yAxisId;

        private Vector2 _current;

        #endregion

        #region CONSTRUCTOR

        public DualAxleInputSignature(string id, string xAxisId, string yAxisId)
            : base(id)
        {
            _xAxisId = xAxisId;
            _yAxisId = yAxisId;
        }

        public DualAxleInputSignature(string id, int hash, string xAxisId, string yAxisId)
            : base(id, hash)
        {
            _xAxisId = xAxisId;
            _yAxisId = yAxisId;
        }

        #endregion

        #region Properties

        public string XAxisId
        {
            get { return _xAxisId; }
            set { _xAxisId = value; }
        }

        public string YAxisId
        {
            get { return _yAxisId; }
            set { _yAxisId = value; }
        }

        public float AxleDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff AxleCutoff
        {
            get;
            set;
        }

        public float RadialDeadZone
        {
            get;
            set;
        }

        public DeadZoneCutoff RadialCutoff
        {
            get;
            set;
        }

        /// <summary>
        /// If you accept keyboard/button input as opposed to analogue input, the values that come out are not radial in nature. 
        /// This will ensure that the button/keyboard inputs acts similar to a joystick.
        /// </summary>
        public bool RadialNormalizeButtonInput
        {
            get;
            set;
        }

        #endregion

        #region IDualAxleInputSignature Interface

        public Vector2 CurrentState
        {
            get { return _current; }
        }

        #endregion

        #region IInputSignature Interface

        public override void Update()
        {
            Vector2 v = new Vector2(Input.GetAxis(_xAxisId), Input.GetAxis(_yAxisId));
            if (this.RadialNormalizeButtonInput && (Input.GetButton(_xAxisId) || Input.GetButton(_yAxisId)))
            {
                if (v.sqrMagnitude > 1f) v.Normalize();
            }
            _current = InputUtil.CutoffDualAxis(v, this.AxleDeadZone, this.AxleCutoff, this.RadialDeadZone, this.RadialCutoff);
        }

        #endregion

    }

    public class MouseCursorInputSignature : AbstractInputSignature, ICursorInputSignature
    {


        public MouseCursorInputSignature(string id)
            : base(id)
        {

        }

        public MouseCursorInputSignature(string id, int hash)
            : base(id, hash)
        {

        }




        public Vector2 CurrentState
        {
            get
            {
                return Input.mousePosition;
            }
        }

        public override void Update()
        {
            //do nothing
        }
    }

    public class MouseClickInputSignature : AbstractInputSignature, IButtonInputSignature
    {

        #region Fields

        private ButtonState _current;
        private ButtonState _currentFixed;

        #endregion

        #region CONSTRUCTOR

        public MouseClickInputSignature(string id, int mouseButton)
            : base(id)
        {
            this.MouseButton = mouseButton;
        }

        public MouseClickInputSignature(string id, int hash, int mouseButton)
            : base(id, hash)
        {
            this.MouseButton = mouseButton;
        }

        #endregion

        #region Properties

        public int MouseButton
        {
            get;
            set;
        }

        #endregion

        #region IButtonInputSignature Interface

        public ButtonState CurrentState
        {
            get
            {
                if (GameLoopEntry.CurrentSequence == UpdateSequence.FixedUpdate)
                {
                    return _currentFixed;
                }
                else
                {
                    return _current;
                }
            }
        }

        public ButtonState GetCurrentState(bool getFixedState)
        {
            return (getFixedState) ? _currentFixed : _current;
        }

        #endregion

        #region IInputSignature Interfacce

        public override void Update()
        {
            //determine based on history
            _current = InputUtil.GetNextButtonState(_current, Input.GetMouseButton(this.MouseButton));
        }

        public override void FixedUpdate()
        {
            //determine based on history
            _currentFixed = InputUtil.GetNextButtonState(_currentFixed, Input.GetMouseButton(this.MouseButton));
        }

        #endregion

    }

}
