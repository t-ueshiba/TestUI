using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using NepGeometryMsgs;
using NepSensorMsgs;
using Nep;

public class PointingVR : MonoBehaviour
{
    // NEP messages to ROS
    public class Pointing
    {
        public Header     header         { get; set; }
        public int        pointing_state { get; set; }  // NO_RES(0): no responses to ROS, SWEEP_RES(1): finger_pos and finger_dir specifies sweep direction, RECAPTURE_RES(2): force ROS to recapture 3D point cloud
        public NepPoint   finger_pos     { get; set; }  // finger position, i.e. right controller
        public NepVector3 finger_dir     { get; set; }  // finger direction, i.e. direction of beam from right controller

        public Pointing()
        {
            header     = new Header();
            finger_pos = new NepPoint();
            finger_dir = new NepVector3();
        }
    }

    // Member variables
    private InputAction  _selectAction;      // GripButton
    private InputAction  _activateAction;    // TriggerButton
    private NepPublisher _pointingPub;
    private static DateTimeOffset UNIX_EPOCH = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

    // Member functions
    private void Start()
    {
        var scene_name = gameObject.scene.name;
        _pointingPub = NepGlobal.Node.NewPub(scene_name + "/pointing", "json", NepGlobal.Conf);    
        Debug.Log("Start publisher[" + scene_name + "/pointing]");

        // Not used but shows how to obtain controller and actions. Use InputAction.ReadValue<TYPE>() for polling action values.
        var controller = GetComponent<ActionBasedController>();
        _selectAction  = controller.selectAction.action;
        _selectAction.Enable();
        _activateAction = controller.activateAction.action;
        _activateAction.Enable(); 
    }

    private void Update()
    {
        var pointing = new Pointing();

        // Get controller position.
        pointing.finger_pos = NepGlobal.UnityToNepPoint(transform.position);
       
        // Get controller rotation and compute its direction.
        pointing.finger_dir = NepGlobal.UnityToNep(transform.TransformDirection(Vector3.forward));
        
        // Get states of controller buttons.
        if (_selectAction.ReadValue<float>() > 0.05f)
            pointing.pointing_state = 2;    // RECAPTURE_RES
        else if (_activateAction.ReadValue<float>() > 0.05f)
            pointing.pointing_state = 1;    // SWEEP_RES
        else
            pointing.pointing_state = 0;    // NO_RES

        var now = (DateTimeOffset.UtcNow - UNIX_EPOCH).Ticks * 100;  // In C#, 1tick = 100nsec
        pointing.header.seq = 0;
        //pointing.header.stamp.sec = (uint)(now / (1000*1000*1000));
        //pointing.header.stamp.nsec = (uint)(now - pointing.header.stamp.sec);
        pointing.header.frame_id = "world";
        
        _pointingPub.Publish(pointing);
    }
}
