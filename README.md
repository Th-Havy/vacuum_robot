# Vacuum robot simulation

A simulation of a vacuum robot in Unity3D and ROS.

## Setup

### Install Unity Packages

* See * [Installing the Unity Robotics packages](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/quick_setup.md)
* [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector#v0.2.0)
* [URDF Importer](https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer)

### Setup ROS endpoint

* Use [docker image for ROS2](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/ros_unity_integration/setup.md#-ros2-environment)
* ```docker build -t foxy -f ros2_docker/Dockerfile .```
* ```docker run -it --rm -p 10000:10000 foxy /bin/bash```
* Make sure to have the docker daemon running (docker desktop on windows).s
* Start the docker with ```ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=0.0.0.0```

## Assets

* Unity URP Example Assets
* Tridify HDRP Furniture Pack

## Author

* Thomas Havy

