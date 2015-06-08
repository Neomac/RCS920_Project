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
    /// <summary>
    /// Store the information about the tracked arm
    /// </summary>
    public class ArmTracked
    {
        /// <summary>
        /// The side of the body which is tracked
        /// </summary>
        string side;

        /// <summary>
        /// Position X, Y, Z of the hand
        /// </summary>
        CameraSpacePoint hand;

        /// <summary>
        /// Position X, Y, Z of the wrist
        /// </summary>
        CameraSpacePoint wrist;

        /// <summary>
        /// Position X, Y, Z of the elbow
        /// </summary>
        CameraSpacePoint elbow;

        /// <summary>
        /// Position X, Y, Z of the shoulder
        /// </summary>
        CameraSpacePoint shoulder;

        /// <summary>
        /// State (Open, Closed, Lasso, Unknown) of the tracked hand
        /// </summary>
        HandState handState;

        /// <summary>
        /// Angle betzeen the torso and the shoulder elbow vector
        /// </summary>
        double angleTSE;

        /// <summary>
        /// Angle betzeen the shoulder elbow vector and the elbow wrist vector
        /// </summary>
        double angleSEEW;

        /// <summary>
        /// Update the value of the tracked arm
        /// </summary>
        /// <param name="body">The body to track</param>
        public void updateValues(Body body, string inputTrackingMode, string inputSideMode, string inputSide)
        {
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

            //Get joints of the body
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

            //Get the distance in meters 
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

            Vector torso2D = new Vector(Math.Round(cameraSpineBase.X - cameraSpineShoulder.X, 3), Math.Round(cameraSpineBase.Y - cameraSpineShoulder.Y, 3));

            //Round every position values
            handLeft.Position.X = (float)Math.Round(handLeft.Position.X, 3);
            handLeft.Position.Y = (float)Math.Round(handLeft.Position.Y, 3);
            handLeft.Position.Z = (float)Math.Round(handLeft.Position.Z, 3);

            if (inputSideMode == "Auto")
            {
                //Check which arm to track

                //Compute the elbow wrist vector for each side of the body and the torso vector
                Vector3D elbowWristLeft = new Vector3D(Math.Round(cameraWristLeft.X - cameraElbowLeft.X, 3), Math.Round(cameraWristLeft.Y - cameraElbowLeft.Y, 3), Math.Round(cameraWristLeft.Z - cameraElbowLeft.Z, 3));
                Vector3D elbowWristRight = new Vector3D(Math.Round(cameraWristRight.X - cameraElbowRight.X, 3), Math.Round(cameraWristRight.Y - cameraElbowRight.Y, 3), Math.Round(cameraWristRight.Z - cameraElbowRight.Z, 3));
                Vector3D torso = new Vector3D(Math.Round(cameraSpineBase.X - cameraSpineShoulder.X, 3), Math.Round(cameraSpineBase.Y - cameraSpineShoulder.Y, 3), Math.Round(cameraSpineBase.Z - cameraSpineShoulder.Z, 3));

                //Compute the dot product between the the elbow wrist vector and the torso for each side. It tells you if the arm and the torso are parallel
                var scalarEWTLeft = Vector3D.DotProduct(elbowWristLeft, torso);
                var scalarEWTRight = Vector3D.DotProduct(elbowWristRight, torso);

                // Select the arm to track based on the dot product between the elbow wrist vector and the torso vector
                var threshold = 0.12;

                if (Math.Abs(scalarEWTRight) < threshold && Math.Abs(scalarEWTLeft) > threshold)
                {
                    updateValuesArm(body, inputTrackingMode, "Right");
                }
                else if (Math.Abs(scalarEWTLeft) < threshold && Math.Abs(scalarEWTRight) > threshold)
                {
                    updateValuesArm(body, inputTrackingMode, "Left");
                }
                else if (Math.Abs(scalarEWTRight) < threshold && Math.Abs(scalarEWTLeft) < threshold)
                {
                    updateValuesArm(body, inputTrackingMode, "Right");
                }
                else
                {
                    updateValuesArm(body, inputTrackingMode, "None");
                }
            }

            if (inputSideMode == "Manual")
            {
                if (inputSide == "Right")
                {
                    updateValuesArm(body, inputTrackingMode, "Right");
                }
                if (inputSide == "Left")
                {
                    updateValuesArm(body, inputTrackingMode, "Left");
                }
            }

            if (this.side == "Right")
            {
                Vector shoulderElbowRight2D = new Vector(Math.Round(cameraShoulderRight.X - cameraElbowRight.X, 3), Math.Round(cameraShoulderRight.Y - cameraElbowRight.Y, 3));
                Vector elbowWristRight2D = new Vector(Math.Round(cameraElbowRight.X - cameraWristRight.X, 3), Math.Round(cameraElbowRight.Y - cameraWristRight.Y, 3));

                angleTSE = Vector.AngleBetween(torso2D, shoulderElbowRight2D);
                angleSEEW = Vector.AngleBetween(shoulderElbowRight2D, elbowWristRight2D);
            }
            else if (this.side == "Left")
            {
                Vector shoulderElbowLeft2D = new Vector(Math.Round(cameraShoulderLeft.X - cameraElbowLeft.X, 3), Math.Round(cameraShoulderLeft.Y - cameraElbowLeft.Y, 3));
                Vector elbowWristLeft2D = new Vector(Math.Round(cameraElbowLeft.X - cameraWristLeft.X, 3), Math.Round(cameraElbowLeft.Y - cameraWristLeft.Y, 3));

                angleTSE = Vector.AngleBetween(torso2D, shoulderElbowLeft2D);
                angleSEEW = Vector.AngleBetween(shoulderElbowLeft2D, elbowWristLeft2D);
            }
        }

        private void updateValuesArm(Body body, string inputTrackingMode, string inputSide)
        {
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
            if (inputTrackingMode == "Mimicking")
	        {
		        if (inputSide == "Right")
	            {
		            side = "Right";
                    hand = originToShoulderBase(joints, joints[JointType.HandRight].Position);
                    wrist = originToShoulderBase(joints, joints[JointType.WristRight].Position);
                    elbow = originToShoulderBase(joints, joints[JointType.ElbowRight].Position);
                    shoulder = originToShoulderBase(joints, joints[JointType.ShoulderRight].Position);
                    handState = body.HandRightState;
	            }
                else if (inputSide == "Left")
	            {
		            side = "Left";
                    hand = originToShoulderBase(joints, joints[JointType.HandLeft].Position);
                    wrist = originToShoulderBase(joints, joints[JointType.WristLeft].Position);
                    elbow = originToShoulderBase(joints, joints[JointType.ElbowLeft].Position);
                    shoulder = originToShoulderBase(joints, joints[JointType.ShoulderLeft].Position);
                    handState = body.HandLeftState;
	            }
                else
	            {
		            side = "None";
                    hand = new CameraSpacePoint();
                    wrist = new CameraSpacePoint();
                    elbow = new CameraSpacePoint();
                    shoulder = new CameraSpacePoint();
                    handState = new HandState();
	            }
	        }
            if (inputTrackingMode == "MovingObject")
	        {
		        if (inputSide == "Right")
	            {
		            side = "Right";
                    hand = originToShoulderBase(joints, joints[JointType.HandRight].Position);
                    elbow = new CameraSpacePoint();
                    shoulder = new CameraSpacePoint();
                    handState = body.HandLeftState;
	            }
                if (inputSide == "Left")
	            {
		            side = "Left";
                    hand = originToShoulderBase(joints, joints[JointType.HandLeft].Position);
                    elbow = new CameraSpacePoint();
                    shoulder = new CameraSpacePoint();
                    handState = body.HandRightState;
	            }
                if (inputSide == "None")
	            {
		            side = "None";
                    hand = new CameraSpacePoint();
                    wrist = new CameraSpacePoint();
                    elbow = new CameraSpacePoint();
                    shoulder = new CameraSpacePoint();
                    handState = new HandState();
	            }
	        }
        }

        public CameraSpacePoint originToShoulderBase(IReadOnlyDictionary<JointType, Joint> joints, CameraSpacePoint cameraSpacePointInput)
        {
            CameraSpacePoint cameraSpacePointOutput = new CameraSpacePoint();
            if (side == "Left")
            {
                cameraSpacePointOutput.X = cameraSpacePointInput.X - joints[JointType.ShoulderLeft].Position.X;
                cameraSpacePointOutput.Y = cameraSpacePointInput.Y - joints[JointType.ShoulderLeft].Position.Y;
                cameraSpacePointOutput.Z = cameraSpacePointInput.Z - joints[JointType.ShoulderLeft].Position.Z;
            }
            else if (side == "Right")
            {
                cameraSpacePointOutput.X = cameraSpacePointInput.X - joints[JointType.ShoulderLeft].Position.X;
                cameraSpacePointOutput.Y = cameraSpacePointInput.Y - joints[JointType.ShoulderLeft].Position.Y;
                cameraSpacePointOutput.Z = cameraSpacePointInput.Z - joints[JointType.ShoulderLeft].Position.Z;
            }
            else
            {
                cameraSpacePointOutput.X = 0;
                cameraSpacePointOutput.Y = 0;
                cameraSpacePointOutput.Z = 0;
            }
            return cameraSpacePointOutput;
        }

        //Getters
        public string getSide()
        {
            return side;
        }

        public CameraSpacePoint getHand()
        {
            return hand;
        }

        public CameraSpacePoint getWrist()
        {
            return wrist;
        }

        public CameraSpacePoint getElbow()
        {
            return elbow;
        }

        public CameraSpacePoint getShoulder()
        {
            return shoulder;
        }

        public HandState getHandState()
        {
            return handState;
        }

        public double getAngleTSE()
        {
            return angleTSE;
        }

        public double getAngleSEEW()
        {
            return angleSEEW;
        }
    }
}
