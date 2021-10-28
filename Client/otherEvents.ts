mp.events.add("FreezeLocalPlayer", (freeze: boolean) => {
    mp.players.local.freezePosition(freeze)
});
