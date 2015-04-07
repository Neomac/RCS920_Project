using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace KinectTest
{
    public class ArmTracked
    {
        string side;
        Joint hand;
        Joint wrist;
        Joint elbow;
        Joint shoulder;

        public void updateValues(Body body)
        {
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

            //Check which arm to track
            Joint handLeft = joints[JointType.HandLeft];
            Joint wristLeft = joints[JointType.WristLeft];
            Joint elbowLeft = joints[JointType.ElbowLeft];
            Joint shoulderLeft = joints[JointType.ShoulderLeft];
            Joint handRight = joints[JointType.HandRight];
            Joint wristRight = joints[JointType.WristRight];
            Joint elbowRight = joints[JointType.ElbowRight];
            Joint shoulderRight = joints[JointType.ShoulderRight];
            Joint head = joints[JointType.Head];
            Joint spineShoulder = joints[JointType.SpineShoulder];
            Joint spineBase = joints[JointType.SpineBase];

            CameraSpacePoint cameraHandLeft = handLeft.Position;
            CameraSpacePoint cameraWristLeft = wristLeft.Position;
            CameraSpacePoint cameraElbowLeft = elbowLeft.Position;
            CameraSpacePoint cameraShoulderLeft = shoulderLeft.Position;
            CameraSpacePoint cameraHandRight = handRight.Position;
            CameraSpacePoint cameraWristRight = wristRight.Position;
            CameraSpacePoint cameraElbowRight = elbowRight.Position;
            CameraSpacePoint cameraShoulderRight = shoulderRight.Position;
            CameraSpacePoint cameraHead = head.Position;
            CameraSpacePoint cameraSpineShoulder = spineShoulder.Position;
            CameraSpacePoint cameraSpineBase = spineBase.Position;

            Vector3D shoulderElbowLeft = new Vector3D(Math.Round(cameraElbowLeft.X - cameraShoulderLeft.X, 3), Math.Round(cameraElbowLeft.Y - cameraShoulderLeft.Y, 3), Math.Round(cameraElbowLeft.Z - cameraShoulderLeft.Z, 3));
            Vector3D elbowWristLeft = new Vector3D(Math.Round(cameraWristLeft.X - cameraElbowLeft.X, 3), Math.Round(cameraWristLeft.Y - cameraElbowLeft.Y, 3), Math.Round(cameraWristLeft.Z - cameraElbowLeft.Z, 3));
            Vector3D shoulderElbowRight = new Vector3D(Math.Round(cameraElbowRight.X - cameraShoulderRight.X, 3), Math.Round(cameraElbowRight.Y - cameraShoulderRight.Y, 3), Math.Round(cameraElbowRight.Z - cameraShoulderRight.Z, 3));
            Vector3D elbowWristRight = new Vector3D(Math.Round(cameraWristRight.X - cameraElbowRight.X, 3), Math.Round(cameraWristRight.Y - cameraElbowRight.Y, 3), Math.Round(cameraWristRight.Z - cameraElbowRight.Z, 3));
            Vector3D torso = new Vector3D(Math.Round(cameraSpineBase.X - cameraSpineShoulder.X, 3), Math.Round(cameraSpineBase.Y - cameraSpineShoulder.Y, 3), Math.Round(cameraSpineBase.Z - cameraSpineShoulder.Z, 3));

            var scalarSEEWLeft = Vector3D.DotProduct(shoulderElbowLeft, elbowWristLeft);
            var scalarEWTLeft = Vector3D.DotProduct(elbowWristLeft, torso);
            var scalarSEEWRight = Vector3D.DotProduct(shoulderElbowRight, elbowWristRight);
            var scalarEWTRight = Vector3D.DotProduct(elbowWristRight, torso);

            if (Math.Abs(scalarEWTRight) < 0.12 && Math.Abs(scalarEWTLeft) > 0.12)
            {
                side = "Right";
                hand = handRight;
                wrist= wristRight;
                elbow= elbowRight;
                shoulder= shoulderRight;
            }
            else
            {
                if (Math.Abs(scalarEWTLeft) < 0.12 && Math.Abs(scalarEWTRight) > 0.12)
                {
                    side = "Left";
                    hand = handLeft;
                    wrist = wristLeft;
                    elbow = elbowLeft;
                    shoulder = shoulderLeft;
                }
                else
                {
                    if (Math.Abs(scalarEWTRight) < 0.12 && Math.Abs(scalarEWTLeft) < 0.12)
                    {
                        side = "Right";
                        hand = handRight;
                        wrist = wristRight;
                        elbow = elbowRight;
                        shoulder = shoulderRight;
                    }
                    else
                    {
                        side = "None";
                        hand = new Joint();
                        wrist = new Joint();
                        elbow = new Joint();
                        shoulder = new Joint();
                    }
                }
            }
        }

        public string getSide()
        {
            return side;
        }

        public Joint getHand()
        {
            return hand;
        }

        public Joint getWrist()
        {
            return wrist;
        }

        public Joint getElbow()
        {
            return elbow;
        }

        public Joint getShoulder()
        {
            return shoulder;
        }


    }
}
