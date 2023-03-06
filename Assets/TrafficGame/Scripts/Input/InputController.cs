using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TrafficGame.Scripts.Input
{
    public sealed class InputController : IDisposable
    {
        private readonly InputActions _inputActions;
        
        [CanBeNull] private IPointerHandler _pressedObject;

        private Camera _camera;

        public InputController(Camera camera)
        {
            _camera = camera;
            _inputActions = new InputActions();
        }

        public void Enable()
        {
            _inputActions.Enable();
            _inputActions.DefaultActionMap.Press.performed += OnPressedPerformed;
            _inputActions.DefaultActionMap.Press.canceled += OnPressCanceled;
        }

        public void Disable()
        {
            _inputActions.Disable();
            _inputActions.DefaultActionMap.Press.performed -= OnPressedPerformed;
            _inputActions.DefaultActionMap.Press.canceled -= OnPressCanceled;
        }

        private void OnPressedPerformed(InputAction.CallbackContext callback)
        {
            var pointerPosition = Pointer.current.position.ReadValue();
            
            if(TryGetPointerHandlerByRaycast(_camera, pointerPosition, out var pointedObject)) 
                _pressedObject = pointedObject;
        }

        private void OnPressCanceled(InputAction.CallbackContext callback)
        {
            // Ignore of no any object was pressed earlier
            if (ReferenceEquals(_pressedObject, null)) return;

            var pointerPosition = Pointer.current.position.ReadValue();
            
            if (!TryGetPointerHandlerByRaycast(_camera, pointerPosition, out var pointedObject)) return;

            // Call click only if press canceled on the same object that was pressed earlier
            if (pointedObject == _pressedObject)
            {
                _pressedObject.OnClicked();
                _pressedObject = null;
            }
            else
            {
                _pressedObject = null;
            }
        }

        private bool TryGetPointerHandlerByRaycast(Camera camera, Vector3 pointerPosition, out IPointerHandler pointerHandler)
        {
            var ray = camera.ScreenPointToRay(pointerPosition);

            // No any object that was hit
            if (!Physics.Raycast(ray, out var hit, camera.farClipPlane))
            {
                pointerHandler = null;
                return false;
            }

            var pointedGameObject = hit.collider.gameObject;

            // Ignore if it's not pointer handler
            if (!pointedGameObject.TryGetComponent(out IPointerHandler pointedObject))
            {
                pointerHandler = null;
                return false;
            }

            pointerHandler = pointedObject;
            return true;
        }

        public void Dispose()
        {
            Disable();
            _inputActions?.Dispose();
            _pressedObject = null;
        }
    }
}