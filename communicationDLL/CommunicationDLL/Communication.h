#ifndef ABBINTERPRETER_H
#define ABBINTERPRETER_H

#include <iostream>
#include <string>
#include "math.h"
#include <cstdio>

using namespace std;


namespace abb_comm
{
	extern "C" { __declspec(dllexport) string pingRobot(int idCode = 0); }
	extern "C" { __declspec(dllexport) string setCartesian(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setJoints(double joint1, double joint2, double joint3, double joint4, double joint5, double joint6, int idCode = 0); }
	extern "C" { __declspec(dllexport) string getCartesian(int idCode = 0); }
	extern "C" { __declspec(dllexport) string getJoints(int idCode = 0); }
	extern "C" { __declspec(dllexport) string setTool(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setWorkObject(double x, double y, double z, double q0, double qx, double qy, double qz, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setSpeed(double tcp, double ori, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setZone(bool fine = 0, double tcp_mm = 5.0, double ori_mm = 5.0, double ori_deg = 1.0, int idCode = 0); }
	extern "C" { __declspec(dllexport) string specialCommand(int command, double param1, double param2, double param3, double param4, double param5, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setVacuum(int vacuum = 0, int idCode = 0); }
	extern "C" { __declspec(dllexport) string setDIO(int dio_number = 0, int dio_state = 0, int idCode = 0); }
	extern "C" { __declspec(dllexport) string closeConnection(int idCode = 0); }
	extern "C" { __declspec(dllexport) int parseCartesian(string msg, double *x, double *y, double *z,
		double *q0, double *qx, double *qy, double *qz); }
	extern "C" { __declspec(dllexport) int parseJoints(string msg, double *joint1, double *joint2,
		double *joint3, double *joint4, double *joint5, double *joint6); }
}
#endif
