mp.events.add('render', () => {
    if(isTestDriveEnabled){
        if(mp.players.local.isInAnyVehicle(false)){
            mp.game.controls.disableControlAction(27, 75, true);
            if(mp.players.local.vehicle.getBodyHealth() < 1000){
                mp.events.callRemote("EmergencyTestDriveStop");
            }
        }
    }
});
