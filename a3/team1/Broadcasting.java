package team1;

import battlecode.common.GameActionException;
import battlecode.common.MapLocation;
import battlecode.common.RobotController;

public class Broadcasting {
	
	private RobotController rc;
	
	public Broadcasting(RobotController rc) {
		this.rc = rc;
	}
	
	public void sendInt(int type, int message) throws GameActionException {
		rc.broadcast(type, message);
	}
	
	public int readInt(int type) throws GameActionException {
		return rc.readBroadcast(type);
	}
	
	public void sendLocation(int type, MapLocation loc) throws GameActionException {
		rc.broadcast(type, loc.x);
		rc.broadcast(type+1, loc.y);
	}
	
	public MapLocation readLocation(int type) throws GameActionException {
		int x = rc.readBroadcast(type);
		int y = rc.readBroadcast(type+1);
		return new MapLocation(x,y);
	}
	

}
