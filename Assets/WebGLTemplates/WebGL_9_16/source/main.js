function UnityOnReady(){
    console.log("Unity đã load xong");
    nativeSide.unityDone();
}
function UnitiSetSound(isOn) {
    console.log("UnitiSetSound", isOn)
    //tắc mở âm thanh trong unity 1 (mở), 0 (tắc)
    if (isOn) { window.unityInstance.SendMessage("GameManager", 'SetSound', 1); }
    else { window.unityInstance.SendMessage("GameManager", 'SetSound', 0); }
}
//đẩy data vô unity
function UnitySendData(data){
    console.log("dataFormNative", data);
    window.unityInstance.SendMessage("GameManager", 'SetDataFromNative', data);            
    // { Token : "", LinkAPI: "", PlayTime: 0, MaxTime: 0, TotalPlay: "" }    
}
//gọi unity mở game
function UnityTriggerPlay(){
    console.log("Trigger play từ native");    
    window.unityInstance.SendMessage("GameManager", 'TriggerNewSession');
}
function UnitySetInstantWin(isInstant){
    //set dạng game intant win trong unity: 1 (instantWin), 0 (thường)
    console.log("SetInstantWin: " + isInstant);    
    window.unityInstance.SendMessage("GameManager", 'SetInstantGame', isInstant);
}
//unity gởi kết quả game
function NativeSendScore(score){
    console.log("Tổng điểm: " + score);
    nativeSide.sendScore(score);
}
function NativeSoundTrigger(numSound) {    
    nativeSide.setSound(numSound);
}
function NativeCallHome(){
    console.log("player hết lượt và mở home scene");
    nativeSide.backHome();
}
function NativeUpdateLife(life){
    console.log("update lượt chơi còn lại của Player: " + life);
    nativeSide.setPlayTimeRemain(life);
}
//---new---
//unity send signal for native to play game
function UnitySendStartGameSignal(){
    nativeSide.startApiGame();
}
//unity get response from "new-play" api (if native post api success)
function UnityGetPlayResponse(response){
    window.unityInstance.SendMessage("GameManager", 'GetPlayResponseFromNative', JSON.stringify(response));
}
//unity send signature for "new-finish" api
function UnitySendSignature(signature){
    nativeSide.finishApiGame(signature);
}
//unity get signal when native post "new-finish" api success
function NativePostFinishSuccess(){
    window.unityInstance.SendMessage("GameManager", 'PostFinishSuccess');
}
//unity send signal for native when fortune game has gift and finish
function ShowWinInstantScene(){
    nativeSide.finishWinInstant();
}

