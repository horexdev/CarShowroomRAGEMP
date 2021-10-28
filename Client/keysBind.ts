// E
mp.keys.bind(0x45, false, () => {
    let isOnShowroomColShape: boolean = mp.players.local.getVariable("IsOnShowroomColShape");

    if (isOnShowroomColShape && !showroom.isMenuAlreadyOpen) {
        showroom.openMenu();
    }
});

// ESC
mp.keys.bind(0x1B, false, () => {
    let isOnShowroomColShape: boolean = mp.players.local.getVariable("IsOnShowroomColShape");

    if (isOnShowroomColShape && showroom.isMenuAlreadyOpen) {
        showroom.closeMenu();
    }
});

// LEFT
mp.keys.bind(0x25, false, () => {
    let isOnShowroomColShape: boolean = mp.players.local.getVariable("IsOnShowroomColShape");

    if (isOnShowroomColShape && showroom.isMenuAlreadyOpen) {
        showroom.changeVehicle(0);
    }
});

// RIGHT
mp.keys.bind(0x27, false, () => {
    let isOnShowroomColShape: boolean = mp.players.local.getVariable("IsOnShowroomColShape");

    if (isOnShowroomColShape && showroom.isMenuAlreadyOpen) {
        showroom.changeVehicle(1);
    }
});
