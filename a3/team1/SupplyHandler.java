package team1;

import team1.Robots.HQRobot;
import battlecode.common.*;

public class SupplyHandler  {

    public static void shareSupply(Robot robot) throws GameActionException {
        RobotInfo[] nearbyAllies = robot.rc.senseNearbyRobots(GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED, robot.myTeam);

        double mySupply = robot.rc.getSupplyLevel();
        int myUpkeep = robot.rc.getType().supplyUpkeep;
        double myTurnsOfSupplyLeft = mySupply / myUpkeep;

        if (myTurnsOfSupplyLeft < 4) return; // no sense spending bytecodes sharing if there's not much to share

        RobotInfo allyToSupply = null;
        double minTurnsOfSupplyLeft = myTurnsOfSupplyLeft;
        for (RobotInfo ally : nearbyAllies) {
            if (needsSupply(ally.type)) {
                double allyTurnsOfSupplyLeft = ally.supplyLevel / ally.type.supplyUpkeep;
                if (allyTurnsOfSupplyLeft < minTurnsOfSupplyLeft) {
                    minTurnsOfSupplyLeft = allyTurnsOfSupplyLeft;
                    allyToSupply = ally;
                }
            }
        }

        if (allyToSupply != null) {
            double allySupply = allyToSupply.supplyLevel;
            int allyUpkeep = allyToSupply.type.supplyUpkeep;

            // we solve: (my supply - x) / (my upkeep) = (ally supply + x) / (ally upkeep)
            double transferAmount = (mySupply * allyUpkeep - allySupply * myUpkeep) / (myUpkeep + allyUpkeep);

            if (transferAmount > 20) {
                // Debug.indicate("supply", 2, "transferring " + (int) transferAmount + " to " + allyToSupply.location.toString() + " to even things up");
                robot.rc.transferSupplies((int) transferAmount, allyToSupply.location);
            }
        }
    }

    // turn mod 3 = 0 -> bots compete to determine max supply need
    // turn mod 3 = 1 -> resupply drone(s) read max supply need
    // turn mod 3 = 2 -> resupply drone(s) reset max supply need comms channels

    static int numTurnsSupplyRequestUnfulfilled = 0;

    public static void requestResupplyIfNecessary(Robot robot) throws GameActionException {
        if (Clock.getRoundNum() % 3 == 0) return; // can only request supply on certain turns

        double travelTimeFromHQ = Math.sqrt(robot.location.distanceSquaredTo(robot.home));

        // lookaheadTurns increases as our supply request remains unfulfilled, giving
        // us higher and higher priority over time
        double lookaheadTurns = 2.0 * travelTimeFromHQ;
        int mySupplyNeeded = (int) (lookaheadTurns * robot.rc.getType().supplyUpkeep - robot.rc.getSupplyLevel());

        if (mySupplyNeeded <= 0) {
            numTurnsSupplyRequestUnfulfilled = 0;
            return;
        } else {
            numTurnsSupplyRequestUnfulfilled++;
        }

        int totalSupplyUpkeepNearby = robot.rc.getType().supplyUpkeep;
        double totalSupplyNearby = robot.rc.getSupplyLevel();

        RobotInfo[] nearbyAllies = robot.rc.senseNearbyRobots(GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED, robot.myTeam);
        for (RobotInfo ally : nearbyAllies) {
            totalSupplyUpkeepNearby += ally.type.supplyUpkeep;
            totalSupplyNearby += ally.supplyLevel;
        }

        int supplyRequestSize = (int) ((lookaheadTurns + numTurnsSupplyRequestUnfulfilled) * totalSupplyUpkeepNearby - totalSupplyNearby);

        // Debug.indicate("supply", 0, " supplyUpkeepNearby = " + totalSupplyUpkeepNearby + "; supplyNearby = " + totalSupplyNearby);
        // Debug.indicate("supply", 1, "supply requestSize: " + supplyRequestSize + "; turns unfulfilled = " + numTurnsSupplyRequestUnfulfilled);

       
        
        if (supplyRequestSize > robot.broadcast.readInt(Parameters.BROAD_SUPPLY_MAX_NEEDED)) {
        	robot.broadcast.sendInt(Parameters.BROAD_SUPPLY_MAX_NEEDED, supplyRequestSize);
        	robot.broadcast.sendLocation(Parameters.BROAD_MOST_NEEDED_SUPPLY_LOCATION, robot.location);
            //MessageBoard.NEEDIEST_SUPPLY_LOC.writeMapLocation(location);
        }
    }

    static MapLocation supplyRunnerDest = null;
    static double supplyRunnerNeed = 0;
    static boolean onSupplyRun = true;

    static MapLocation supplyRunnerLastLoc = null;
    static int supplyRunnerTurnsSinceMove = 0;

    public static void runSupplies(Robot robot) throws GameActionException {
        if (robot.location.equals(supplyRunnerLastLoc)) {
            supplyRunnerTurnsSinceMove++;
            if (supplyRunnerTurnsSinceMove >= 50 && robot.location.distanceSquaredTo(robot.home) > GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED) {
                // System.out.println("supply runner disintegrating");
                robot.rc.disintegrate();
            }
        } else {
            supplyRunnerTurnsSinceMove = 0;
        }
        supplyRunnerLastLoc = robot.location;

        // read supply needs
        if (Clock.getRoundNum() % 3 == 1) {
        	
            supplyRunnerNeed = robot.broadcast.readInt(Parameters.BROAD_SUPPLY_MAX_NEEDED);
            if (supplyRunnerNeed > 0) {
            	supplyRunnerDest = robot.broadcast.readLocation(Parameters.BROAD_MOST_NEEDED_SUPPLY_LOCATION);
                // Debug.indicate("supply", 0, "max supply needed = " + supplyRunnerNeed + " at " + supplyRunnerDest.toString());
            } else {
                supplyRunnerDest = null;
                // Debug.indicate("supply", 0, "no supply need");
            }
        }

        // reset supply need comms channels
        if (Clock.getRoundNum() % 3 == 2) {
        	robot.broadcast.sendInt(Parameters.BROAD_SUPPLY_MAX_NEEDED, 0);
        }

        if (supplyRunnerDest != null && robot.location.distanceSquaredTo(supplyRunnerDest) < 35) {
            if (supplyRunnerTransferSupplyAtDest(robot)) {
                onSupplyRun = false; // supplies have been dropped off; return to HQ
            }
        } else {
            // try helping out whoever we encounter on the way to the main destination
            supplyRunnerTryOpportunisticTransferSupply(robot);
        }

        if (onSupplyRun) {
            // call off a supply run if the need vanishes or if we run out of spare supply
            if (supplyRunnerNeed == 0 || supplyRunnerSpareSupplyAmount(robot) <= 0) {
                onSupplyRun = false;
            }
        } else {
            // start a supply run when there is need and we have enough supply to fulfill it
            if (supplyRunnerNeed > 0) {
                // double supplyNeededForRun = supplyRunnerNeed + RobotType.DRONE.supplyUpkeep * Math.sqrt(ourHQ.distanceSquaredTo(supplyRunnerDest));
                double supplyNeededForRun = 3 * RobotType.DRONE.supplyUpkeep * Math.sqrt(robot.home.distanceSquaredTo(supplyRunnerDest));
                if (robot.rc.getSupplyLevel() > supplyNeededForRun) {
                    onSupplyRun = true;
                } else {
                    // rc.setIndicatorLine(here, supplyRunnerDest, 255, 0, 0);
                }
            }
        }

        MapLocation[] enemyTowers = robot.rc.senseEnemyTowerLocations();
        RobotInfo[] nearbyEnemies = robot.rc.senseNearbyRobots(35, robot.enemyTeam);

        if (supplyRunnerRetreatIfNecessary(nearbyEnemies, enemyTowers, robot)) return;
        
        
        
        Action.tryMove(robot.location.directionTo(supplyRunnerDest), robot.rc);

//        NavSafetyPolicy safetyPolicy = new SafetyPolicyAvoidAllUnits(enemyTowers, nearbyEnemies);
//        if (onSupplyRun) {
//            Nav.goTo(supplyRunnerDest, safetyPolicy);
//            // Debug.indicate("supply", 2, "going to supply dest");
//            // rc.setIndicatorLine(here, supplyRunnerDest, 0, 255, 0);
//        } else {
//            Nav.goTo(robot.home, safetyPolicy);
//            // Debug.indicate("supply", 2, "returning to HQ");
//        }
    }

    private static boolean supplyRunnerTransferSupplyAtDest(Robot robot) throws GameActionException {
        int transferAmount = (int) supplyRunnerSpareSupplyAmount(robot);
        if (transferAmount <= 0) return true; // we didn't succeed but we are out of supply so it's like we succeeded

        RobotInfo[] nearbyAllies = robot.rc.senseNearbyRobots(GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED, robot.myTeam);

        double minSupply = 1e99;
        RobotInfo allyToSupply = null;
        for (RobotInfo ally : nearbyAllies) {
            if (needsSupply(ally.type)) {
                if (ally.supplyLevel < minSupply) {
                    minSupply = ally.supplyLevel;
                    allyToSupply = ally;
                }
            }
        }

        if (allyToSupply != null) {
            // Debug.indicate("supply", 1, "dropping off " + transferAmount + " supplies at destination");
            robot.rc.transferSupplies(transferAmount, allyToSupply.location);
            return true;
        } else {
            return false;
        }
    }

    // We're not at our main supply destination, but we give people we encounter
    // on the way however much they need. However we don't give them all of our
    // supply because we are saving it for the main destination.
    private static void supplyRunnerTryOpportunisticTransferSupply(Robot robot) throws GameActionException {
        RobotInfo[] nearbyAllies = robot.rc.senseNearbyRobots(GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED, robot.myTeam);

        double travelTimeFromHQ = Math.sqrt(robot.location.distanceSquaredTo(robot.home));

        double transferAmount = 0;
        RobotInfo transferTarget = null;
        for (RobotInfo ally : nearbyAllies) {
            if (needsSupply(ally.type)) {
                double supplyNeed = 2 * travelTimeFromHQ * ally.type.supplyUpkeep - ally.supplyLevel;
                if (supplyNeed > transferAmount) {
                    transferAmount = supplyNeed;
                    transferTarget = ally;
                }
            }
        }

        if (transferTarget != null) {
            transferAmount = Math.min(transferAmount, supplyRunnerSpareSupplyAmount(robot));
            if (transferAmount > 1) {
                // Debug.indicate("supply", 1, "opportunistically transferring " + transferAmount + " to " + transferTarget.location);
                robot.rc.transferSupplies((int) transferAmount, transferTarget.location);
            }
        }
    }

    private static double supplyRunnerSpareSupplyAmount(Robot robot) {
        return robot.rc.getSupplyLevel() - 2 * RobotType.DRONE.supplyUpkeep * Math.sqrt(robot.location.distanceSquaredTo(robot.home));
    }

    private static boolean needsSupply(RobotType rt) {
        return !rt.isBuilding && rt != RobotType.BEAVER && rt != RobotType.MISSILE && rt != RobotType.DRONE;
    }

    private static boolean needToRetreat(RobotInfo[] nearbyEnemies, MapLocation location) {
        for (RobotInfo enemy : nearbyEnemies) {
            switch (enemy.type) {
                case MISSILE:
                    if (location.distanceSquaredTo(enemy.location) <= 15) return true;
                    break;

                case LAUNCHER:
                    if (location.distanceSquaredTo(enemy.location) <= 24) return true;
                    break;

                default:
                    if (enemy.type.attackRadiusSquared >= location.distanceSquaredTo(enemy.location)) return true;
                    break;
            }
        }
        return false;
    }

    private static boolean supplyRunnerRetreatIfNecessary(RobotInfo[] nearbyEnemies, MapLocation[] enemyTowers, Robot robot) throws GameActionException {
        if (!needToRetreat(nearbyEnemies, robot.location)) return false;

        Direction bestRetreatDir = null;
        RobotInfo currentClosestEnemy = Util.closest(nearbyEnemies, robot.location);

        boolean mustMoveOrthogonally = false;
        if (robot.rc.getCoreDelay() >= 0.6 && currentClosestEnemy.type == RobotType.MISSILE) mustMoveOrthogonally = true;

        int bestDistSq = robot.location.distanceSquaredTo(currentClosestEnemy.location);
        for (Direction dir : Direction.values()) {
            if (!robot.rc.canMove(dir)) continue;
            if (mustMoveOrthogonally && dir.isDiagonal()) continue;

            MapLocation retreatLoc = robot.location.add(dir);
            if (robot.inEnemyTowerOrHQRange(retreatLoc, enemyTowers)) continue;
            

            RobotInfo closestEnemy = Util.closest(nearbyEnemies, retreatLoc);
            int distSq = retreatLoc.distanceSquaredTo(closestEnemy.location);
            if (distSq > bestDistSq) {
                bestDistSq = distSq;
                bestRetreatDir = dir;
            }
        }

        if (bestRetreatDir != null) {
            robot.rc.move(bestRetreatDir);
            return true;
        }
        return false;
    }

    static double hqLastSupply = 0;
    static double hqSupplyReservedForResupplyDrone = 0;
    static final double HQ_SUPPLY_DRONE_RESERVE_RATIO = 0.5;
    static final double HQ_SUPPLY_TURN_BUILDUP_LIMIT = 100.0;

    public static void hqGiveSupply(Robot robot) throws GameActionException {
        // reserve a fraction of the supply just generated for the resupply drone
        hqSupplyReservedForResupplyDrone += HQ_SUPPLY_DRONE_RESERVE_RATIO * HQRobot.totalSupplyGenerated;

        // need to make sure vast amounts of supply don't build up unused if only the supply drone is visiting the HQ.
        // so every turn a fraction of the unreserved supply is reserved for the drone
        hqSupplyReservedForResupplyDrone += (robot.rc.getSupplyLevel() - hqSupplyReservedForResupplyDrone) / HQ_SUPPLY_TURN_BUILDUP_LIMIT;

        // Debug.indicate("supply", 0, "supply reserved for drone = " + hqSupplyReservedForResupplyDrone);

        // feed the resupply drone if it's around
        RobotInfo resupplyDrone = null;
        RobotInfo[] nearbyAllies = robot.rc.senseNearbyRobots(GameConstants.SUPPLY_TRANSFER_RADIUS_SQUARED, robot.myTeam);
        for (RobotInfo ally : nearbyAllies) {
            if (ally.type == RobotType.DRONE) {
                if (resupplyDrone == null || ally.supplyLevel > resupplyDrone.supplyLevel) {
                    resupplyDrone = ally;
                }
            }
        }

        if (resupplyDrone != null) {
            // Debug.indicate("supply", 1, "transferring " + hqSupplyReservedForResupplyDrone + " to resupply drone");
            robot.rc.transferSupplies((int) hqSupplyReservedForResupplyDrone, resupplyDrone.location);
            hqSupplyReservedForResupplyDrone = 0;
        }

        // feed nearby robots whatever supply is not reserved for the resupply drone
        double minTurnsOfSupplyLeft = 1e99;
        RobotInfo allyToSupply = null;
        for (RobotInfo ally : nearbyAllies) {
            if (needsSupply(ally.type)) {
                double allyTurnsOfSupplyLeft = ally.supplyLevel / ally.type.supplyUpkeep;
                if (allyTurnsOfSupplyLeft < minTurnsOfSupplyLeft) {
                    minTurnsOfSupplyLeft = allyTurnsOfSupplyLeft;
                    allyToSupply = ally;
                }
            }
        }

        if (allyToSupply != null) {
            int transferAmount = (int) (robot.rc.getSupplyLevel() - hqSupplyReservedForResupplyDrone);
            if (transferAmount > 0) {
                // Debug.indicate("supply", 2, "transferring " + transferAmount + " to " + allyToSupply.location.toString());
                robot.rc.transferSupplies(transferAmount, allyToSupply.location);
            }
        } else {
            // Debug.indicate("supply", 0, "no non-drone to supply :(");
        }
    }
}