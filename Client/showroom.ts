class Showroom {
    isMenuAlreadyOpen: boolean = false;

    public openMenu(): void {
        mp.players.local.freezePosition(true)

        mp.events.callRemote("OpenShowroomMenu");
        mp.events.call("ActivateCamera", true);

        this.isMenuAlreadyOpen = true;
    }

    public closeMenu(): void {
        mp.players.local.freezePosition(false)

        mp.events.callRemote("CloseShowroomMenu");
        mp.events.call("ActivateCamera", false);

        this.isMenuAlreadyOpen = false;
    }

    // 0 - Лево, 1 - Право
    public changeVehicle(way: number): void {
        mp.events.callRemote("ChangeShowroomVehicle", way);
    }
}

let showroom: Showroom = new Showroom();
let isTestDriveEnabled: boolean = false;
let cameras = [mp.cameras.new('DEFAULT_SCRIPTED_CAMERA',
    new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), 30)]

mp.events.add("OpenShowroomMenu", () => {
   showroom.openMenu();
});

mp.events.add("CloseShowroomMenu", () => {
    showroom.closeMenu();
});

mp.events.add("SetCamera", (x_pos: number, y_pos: number, z_pos: number,
                             x_rot: number, y_rot: number, z_rot: number) => {
    cameras[0] = mp.cameras.new('DEFAULT_SCRIPTED_CAMERA',
        new mp.Vector3(x_pos, y_pos, z_pos), new mp.Vector3(x_rot, y_rot, z_rot), 30);
});

mp.events.add("ActivateCamera", (state: boolean) => {
    cameras[0].setActive(state);
    mp.game.cam.renderScriptCams(state, false, 0, false, false);
});

mp.events.add("TestDriveStatus", (isActive: boolean) => {
    isTestDriveEnabled = isActive
});

mp.events.add("SendVehicle", (entity: VehicleMp) => {
    vehicle = entity;
})
