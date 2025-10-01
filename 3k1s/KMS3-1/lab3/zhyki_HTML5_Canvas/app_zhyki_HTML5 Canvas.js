(function (cjs, an) {

var p; // shortcut to reference prototypes
var lib={};var ss={};var img={};
lib.ssMetadata = [];


(lib.AnMovieClip = function(){
	this.actionFrames = [];
	this.ignorePause = false;
	this.currentSoundStreamInMovieclip;
	this.soundStreamDuration = new Map();
	this.streamSoundSymbolsList = [];

	this.gotoAndPlayForStreamSoundSync = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.gotoAndPlay = function(positionOrLabel){
		this.clearAllSoundStreams();
		var pos = this.timeline.resolve(positionOrLabel);
		if (pos != null) { this.startStreamSoundsForTargetedFrame(pos); }
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.play = function(){
		this.clearAllSoundStreams();
		this.startStreamSoundsForTargetedFrame(this.currentFrame);
		cjs.MovieClip.prototype.play.call(this);
	}
	this.gotoAndStop = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndStop.call(this,positionOrLabel);
		this.clearAllSoundStreams();
	}
	this.stop = function(){
		cjs.MovieClip.prototype.stop.call(this);
		this.clearAllSoundStreams();
	}
	this.startStreamSoundsForTargetedFrame = function(targetFrame){
		for(var index=0; index<this.streamSoundSymbolsList.length; index++){
			if(index <= targetFrame && this.streamSoundSymbolsList[index] != undefined){
				for(var i=0; i<this.streamSoundSymbolsList[index].length; i++){
					var sound = this.streamSoundSymbolsList[index][i];
					if(sound.endFrame > targetFrame){
						var targetPosition = Math.abs((((targetFrame - sound.startFrame)/lib.properties.fps) * 1000));
						var instance = playSound(sound.id);
						var remainingLoop = 0;
						if(sound.offset){
							targetPosition = targetPosition + sound.offset;
						}
						else if(sound.loop > 1){
							var loop = targetPosition /instance.duration;
							remainingLoop = Math.floor(sound.loop - loop);
							if(targetPosition == 0){ remainingLoop -= 1; }
							targetPosition = targetPosition % instance.duration;
						}
						instance.loop = remainingLoop;
						instance.position = Math.round(targetPosition);
						this.InsertIntoSoundStreamData(instance, sound.startFrame, sound.endFrame, sound.loop , sound.offset);
					}
				}
			}
		}
	}
	this.InsertIntoSoundStreamData = function(soundInstance, startIndex, endIndex, loopValue, offsetValue){ 
 		this.soundStreamDuration.set({instance:soundInstance}, {start: startIndex, end:endIndex, loop:loopValue, offset:offsetValue});
	}
	this.clearAllSoundStreams = function(){
		this.soundStreamDuration.forEach(function(value,key){
			key.instance.stop();
		});
 		this.soundStreamDuration.clear();
		this.currentSoundStreamInMovieclip = undefined;
	}
	this.stopSoundStreams = function(currentFrame){
		if(this.soundStreamDuration.size > 0){
			var _this = this;
			this.soundStreamDuration.forEach(function(value,key,arr){
				if((value.end) == currentFrame){
					key.instance.stop();
					if(_this.currentSoundStreamInMovieclip == key) { _this.currentSoundStreamInMovieclip = undefined; }
					arr.delete(key);
				}
			});
		}
	}

	this.computeCurrentSoundStreamInstance = function(currentFrame){
		if(this.currentSoundStreamInMovieclip == undefined){
			var _this = this;
			if(this.soundStreamDuration.size > 0){
				var maxDuration = 0;
				this.soundStreamDuration.forEach(function(value,key){
					if(value.end > maxDuration){
						maxDuration = value.end;
						_this.currentSoundStreamInMovieclip = key;
					}
				});
			}
		}
	}
	this.getDesiredFrame = function(currentFrame, calculatedDesiredFrame){
		for(var frameIndex in this.actionFrames){
			if((frameIndex > currentFrame) && (frameIndex < calculatedDesiredFrame)){
				return frameIndex;
			}
		}
		return calculatedDesiredFrame;
	}

	this.syncStreamSounds = function(){
		this.stopSoundStreams(this.currentFrame);
		this.computeCurrentSoundStreamInstance(this.currentFrame);
		if(this.currentSoundStreamInMovieclip != undefined){
			var soundInstance = this.currentSoundStreamInMovieclip.instance;
			if(soundInstance.position != 0){
				var soundValue = this.soundStreamDuration.get(this.currentSoundStreamInMovieclip);
				var soundPosition = (soundValue.offset?(soundInstance.position - soundValue.offset): soundInstance.position);
				var calculatedDesiredFrame = (soundValue.start)+((soundPosition/1000) * lib.properties.fps);
				if(soundValue.loop > 1){
					calculatedDesiredFrame +=(((((soundValue.loop - soundInstance.loop -1)*soundInstance.duration)) / 1000) * lib.properties.fps);
				}
				calculatedDesiredFrame = Math.floor(calculatedDesiredFrame);
				var deltaFrame = calculatedDesiredFrame - this.currentFrame;
				if((deltaFrame >= 0) && this.ignorePause){
					cjs.MovieClip.prototype.play.call(this);
					this.ignorePause = false;
				}
				else if(deltaFrame >= 2){
					this.gotoAndPlayForStreamSoundSync(this.getDesiredFrame(this.currentFrame,calculatedDesiredFrame));
				}
				else if(deltaFrame <= -2){
					cjs.MovieClip.prototype.stop.call(this);
					this.ignorePause = true;
				}
			}
		}
	}
}).prototype = p = new cjs.MovieClip();
// symbols:



(lib.остановка = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("ABQk6ICMAAIAAJrIiMAAgAm8m8IN5AAIAAN5It5AAgAjMk6ICMAAIAAJrIiMAAg");
	this.shape.setTransform(-2.5,3.55);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#000000").s().p("ABIE2IAAprICNAAIAAJrgAjUE2IAAprICMAAIAAJrg");
	this.shape_1.setTransform(-1.75,3.05);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f("#999999").s().p("Am8G9IAAt5IN5AAIAAN5gABQExICMAAIAAprIiMAAgAjMExICMAAIAAprIiMAAg");
	this.shape_2.setTransform(-2.5,3.55);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_2},{t:this.shape_1},{t:this.shape}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-48,-41.9,91,91);


(lib.запуск = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("AjCk1IHfE1InfEsgAm8m8IN5AAIAAN5It5AAg");
	this.shape.setTransform(0.5,0.05);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#000000").s().p("AjvkwIHfE1InfEsg");
	this.shape_1.setTransform(5,-0.45);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f("#999999").s().p("Am8G9IAAt5IN5AAIAAN5gAjCEsIHfksInfk1g");
	this.shape_2.setTransform(0.5,0.05);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_2},{t:this.shape_1},{t:this.shape}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-45,-45.4,91,91);


(lib.возврат = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(0.1,1,1).p("AA3ByIhtjj");
	this.shape.setTransform(-6.05,-17.5);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f().s("#000000").ss(11,1,1).p("ABFi9QAPAGAOAJQBGAoAVBOQAVBNgoBHQgpBGhOAVQhNAVhGgpQhHgogVhOQgGgWgBgW");
	this.shape_1.setTransform(-0.0135,-0.054);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f().s("#000000").ss(1,1,1).p("Am8m8IN5AAIAAN5It5AAg");
	this.shape_2.setTransform(0.5,0.05);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f("#000000").s().p("Ah/hlID/gXIhHB6IhKCAg");
	this.shape_3.setTransform(1.225,-18.7);

	this.shape_4 = new cjs.Shape();
	this.shape_4.graphics.f("#999999").s().p("Am8G9IAAt5IN5AAIAAN5gAgGC9QAZAAAagIQBPgUAohHQAbguAAgwQAAgagIgbQgUhPhGgoQgOgIgPgGQAPAGAOAIQBGAoAUBPQAIAbAAAaQAAAwgbAuQgoBHhPAUQgaAIgZAAIAAAAIAAAAQgyAAgtgbQhHgogVhOQgGgXgBgVQABAVAGAXQAVBOBHAoQAtAbAyAAIAAAAIAAAAgAh3kgIBuDjIBJiBIBHh6g");
	this.shape_4.setTransform(0.5,0.05);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_4},{t:this.shape_3},{t:this.shape_2},{t:this.shape_1},{t:this.shape}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-45,-45.4,91,91);


(lib.Тело = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("ANPE+QjiDfjWBRQjWBSjtAuQjuAviYgQQiYgRgEgBQiVgphiiCQiMi5glltQgTi8AeiRQArjOCOh2QAtglC/hBQDAhAFkgDQFjgCCgAXQChAXCuBpQCuBoASD/QBBFzjiDfg");
	this.shape.setTransform(-16.1667,8.4918);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f().s("#000000").ss(1,2,1).p("ApuIHQgUAHgVAGQixAvifhgQiehfgxi3Qgxi2BZiiQBaiiCxgvQA5gQA3AAAmFgOQA9FggvFgApaqwQCbFRA6FRQNkhpLojm");
	this.shape_1.setTransform(-37.7292,15.7612);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f("#660000").s().p("AgpIGQiLi5gllsQgTi8AeiRQArjOCOh2QCaFRA6FRQA9FfgvFgQiVgphhiCg");
	this.shape_2.setTransform(-95.8375,15.8125);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f("#9B0000").s().p("AjhMNQiYgRgEgBQAulhg8lgQNjhpLpjlQrpDltjBpQg6lQiclRQAtglC/hBQDAhAFjgDQFkgCCgAXQChAXCuBpQCuBoASD/QBBFzjiDfQjiDfjWBRQjWBSjuAuQivAjiBAAQgtAAgogEgAvtIsQifhfgxi3Qgxi2BaiiQBaiiCwgvQA5gQA3AAQgeCRATC8QAlFtCMC5QgUAHgWAGQg6APg4AAQhzAAhqhAg");
	this.shape_3.setTransform(-37.0652,8.4918);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_3},{t:this.shape_2},{t:this.shape_1},{t:this.shape}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-161.2,-70.9,248.29999999999998,158.9);


(lib.Лапка = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f("#000000").s().p("AlNBYIBmhPIELgPIBdheIAEAAICQAaIA4AqIjEgpIhZBiIkIAKIhrBCg");
	this.shape.setTransform(33.35,10.125);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#000000").s().p("AkqB9IBQhcID1g9IBFhoIAEAAICKgCIA9AeIi+gEIhBBsIjzA2IhXBRg");
	this.shape_1.setTransform(29.7,6.775);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f("#000000").s().p("AkGCuIA7hoIDehrIAuhxIADgCICEgcIA/AQIi2AiIgpB0IjeBlIhDBeg");
	this.shape_2.setTransform(26.025,2.125);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f("#000000").s().p("AjjDfIAlh0IDIiZIAWh7IADgCIB+g4IBDADIivBIIgRB8IjJCTIgwBtg");
	this.shape_3.setTransform(22.375,-2.55);

	this.shape_4 = new cjs.Shape();
	this.shape_4.graphics.f("#000000").s().p("AjAEWIAQiAICxjJIgBiDIADgDIB4hUIBGgJIioBtIAICFIi0DBIgdB7g");
	this.shape_4.setTransform(18.7,-7.7);

	this.shape_5 = new cjs.Shape();
	this.shape_5.graphics.f("#000000").s().p("AjcDpIAhh2IDDijIASh9IADgCIB9g9IBDAAIiuBQIgLB+IjFCcIgsBvg");
	this.shape_5.setTransform(21.625,-3.475);

	this.shape_6 = new cjs.Shape();
	this.shape_6.graphics.f("#000000").s().p("Aj4DCIAyhtIDVh+IAkh1IADgCICCgnIBBALIizAyIgfB2IjWB4Ig8Bkg");
	this.shape_6.setTransform(24.575,0.25);

	this.shape_7 = new cjs.Shape();
	this.shape_7.graphics.f("#000000").s().p("AkUCaIBDhiIDnhaIA4htIADgBICGgSIA+AWIi5ATIgyBxIjmBSIhMBZg");
	this.shape_7.setTransform(27.475,3.975);

	this.shape_8 = new cjs.Shape();
	this.shape_8.graphics.f("#000000").s().p("AkwB1IBUhZID5g1IBKhlIADAAICMADIA7AhIi+gMIhGBpIj3AvIhbBOg");
	this.shape_8.setTransform(30.425,7.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape}]}).to({state:[{t:this.shape_1}]},1).to({state:[{t:this.shape_2}]},1).to({state:[{t:this.shape_3}]},1).to({state:[{t:this.shape_4}]},1).to({state:[{t:this.shape_5}]},1).to({state:[{t:this.shape_6}]},1).to({state:[{t:this.shape_7}]},1).to({state:[{t:this.shape_8}]},1).to({state:[{t:this.shape}]},1).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-0.6,-35.6,67.3,55.900000000000006);


(lib.Фрагментролика = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.Лапка("synched",0);
	this.instance.setTransform(-32.35,-71.4,1,1,-105.0002,0,0,-5,18.4);

	this.instance_1 = new lib.Лапка("synched",7);
	this.instance_1.setTransform(97.25,35.5,1,1,0,173.0463,-6.9537,-5,18.4);

	this.instance_2 = new lib.Лапка("synched",4);
	this.instance_2.setTransform(47.4,58.65,0.9999,0.9999,0,-141.9536,38.0464,-5,18.4);

	this.instance_3 = new lib.Лапка("synched",1);
	this.instance_3.setTransform(-7.95,70.25,1,1,0,-96.9537,83.0463,-5,18.5);

	this.instance_4 = new lib.Лапка("synched",6);
	this.instance_4.setTransform(73.75,-73.5,1,1,-15.0002,0,0,-5,18.4);

	this.instance_5 = new lib.Лапка("synched",3);
	this.instance_5.setTransform(18.9,-76.3,1,1,-60.0011,0,0,-5,18.4);

	this.instance_6 = new lib.Тело("synched",0);
	this.instance_6.setTransform(-1,-1.95,1,1,0,0,0,-37.1,8.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_6},{t:this.instance_5,p:{startPosition:3}},{t:this.instance_4,p:{startPosition:6}},{t:this.instance_3,p:{startPosition:1}},{t:this.instance_2,p:{startPosition:4}},{t:this.instance_1,p:{startPosition:7}},{t:this.instance,p:{startPosition:0}}]}).to({state:[{t:this.instance_6},{t:this.instance_5,p:{startPosition:8}},{t:this.instance_4,p:{startPosition:8}},{t:this.instance_3,p:{startPosition:8}},{t:this.instance_2,p:{startPosition:8}},{t:this.instance_1,p:{startPosition:8}},{t:this.instance,p:{startPosition:8}}]},9).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-125.1,-144.2,294.9,287);


// stage content:
(lib.app_zhyki_HTML5Canvas = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	this.actionFrames = [0,25];
	this.streamSoundSymbolsList[0] = [{id:"zyk1",startFrame:0,endFrame:60,loop:1,offset:0}];
	this.streamSoundSymbolsList[25] = [{id:"zvukletjaschegozhuka",startFrame:25,endFrame:39,loop:1,offset:0}];
	// timeline functions:
	this.frame_0 = function() {
		this.clearAllSoundStreams();
		 
		var soundInstance = playSound("zyk1",0);
		this.InsertIntoSoundStreamData(soundInstance,0,60,1);
		this.stop(); 
		
		this.btnStop.addEventListener("click", () => {
		    this.stop();
		});
		
		this.btnPlay.addEventListener("click", () => {
		    this.play();
		});
		
		this.btnRewind.addEventListener("click", () => {
		    this.gotoAndStop(0); // возрат к первому кадру (нумерация с 0)
		});
		/* import flash.events.MouseEvent;
		
		stop(); // Остановить анимацию на первом кадре
		
		btnStop.addEventListener(MouseEvent.CLICK, onStopClick);
		btnPlay.addEventListener(MouseEvent.CLICK, onPlayClick);
		btnRewind.addEventListener(MouseEvent.CLICK, onRewindClick);
		
		function onStopClick(e:MouseEvent):void {
		    stop();
		}
		
		function onPlayClick(e:MouseEvent):void {
		    play();
		}
		
		function onRewindClick(e:MouseEvent):void {
		    gotoAndStop(1);
		}
		*/
	}
	this.frame_25 = function() {
		var soundInstance = playSound("zvukletjaschegozhuka",0);
		this.InsertIntoSoundStreamData(soundInstance,25,39,1);
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).call(this.frame_0).wait(25).call(this.frame_25).wait(35));

	// возврат
	this.btnRewind = new lib.возврат();
	this.btnRewind.name = "btnRewind";
	this.btnRewind.setTransform(470.05,453.3,0.4667,0.4667,0,0,0,0.6,0.1);
	new cjs.ButtonHelper(this.btnRewind, 0, 1, 1);

	this.timeline.addTween(cjs.Tween.get(this.btnRewind).wait(60));

	// запуск
	this.btnPlay = new lib.запуск();
	this.btnPlay.name = "btnPlay";
	this.btnPlay.setTransform(416.55,453.7,0.4667,0.4667);
	new cjs.ButtonHelper(this.btnPlay, 0, 1, 1);

	this.timeline.addTween(cjs.Tween.get(this.btnPlay).wait(60));

	// остановка
	this.btnStop = new lib.остановка();
	this.btnStop.name = "btnStop";
	this.btnStop.setTransform(366.15,451.6,0.4667,0.4667);
	new cjs.ButtonHelper(this.btnStop, 0, 1, 1);

	this.timeline.addTween(cjs.Tween.get(this.btnStop).wait(60));

	// жук2
	this.instance = new lib.Фрагментролика();
	this.instance.setTransform(234.65,283.7,0.1662,0.1662,164.9995,0,0,25.2,-9.2);

	this.timeline.addTween(cjs.Tween.get(this.instance).to({guide:{path:[234.6,283.7,235.2,283.5,235.8,283.3,240.9,281.4,246,279.5,247.9,278.9,249.9,278.3,255,276.9,259.9,274.9,264.6,273,269.6,271.9,270.9,271.5,272.2,271.1,276.6,269.4,281.3,268.1,286,266.8,290.9,265.8,295.8,264.7,300.7,263.6,305.3,262.6,309.7,261.2,312,260.5,314.3,260,318.9,258.9,323.4,257.5,328.2,255.9,333,254.7,334.2,254.4,335.3,254.1]}},29).to({regX:24.9,scaleY:0.1875,rotation:0,skewX:-14.9999,skewY:164.9995,guide:{path:[335.3,254.1,335.3,254.1,335.3,254.1]}},5).to({regX:25.2,scaleY:0.1662,rotation:164.9995,skewX:0,skewY:0,guide:{path:[335.3,254.1,335.3,254.1,335.3,254.1]}},5).to({regX:25.1,regY:-9,rotation:135,x:466.5,y:185.95},20).wait(1));

	// жуки
	this.instance_1 = new lib.Фрагментролика();
	this.instance_1.setTransform(519.1,175.15,0.0941,0.0941,180,0,0,25,-8.5);

	this.instance_2 = new lib.Фрагментролика();
	this.instance_2.setTransform(572.9,223.85,0.1663,0.1663,75.0036,0,0,25,-8.6);

	this.instance_3 = new lib.Фрагментролика();
	this.instance_3.setTransform(583.4,451.45,0.3125,0.3125,45.002,0,0,25.4,-8.1);

	this.instance_4 = new lib.Фрагментролика();
	this.instance_4.setTransform(259.4,2.85,0.2868,0.2868,-119.9975,0,0,24.7,-8.5);

	this.instance_5 = new lib.Фрагментролика();
	this.instance_5.setTransform(37.7,353.2,0.1877,0.1877,60.0044,0,0,25.3,-8.2);

	this.instance_6 = new lib.Фрагментролика();
	this.instance_6.setTransform(280.7,442.1,0.1079,0.1079,150.0049,0,0,25.2,-8.4);

	this.instance_7 = new lib.Фрагментролика();
	this.instance_7.setTransform(587.4,131,0.2223,0.2223,-44.9944,0,0,24.9,-8.1);

	this.instance_8 = new lib.Фрагментролика();
	this.instance_8.setTransform(33.4,46.8,0.1663,0.1663,-29.9972,0,0,25,-8.4);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_8,p:{x:33.4,y:46.8}},{t:this.instance_7,p:{x:587.4,y:131}},{t:this.instance_6,p:{x:280.7,y:442.1}},{t:this.instance_5,p:{x:37.7,y:353.2}},{t:this.instance_4,p:{x:259.4,y:2.85}},{t:this.instance_3,p:{x:583.4,y:451.45}},{t:this.instance_2,p:{x:572.9,y:223.85}},{t:this.instance_1,p:{x:519.1,y:175.15}}]}).to({state:[{t:this.instance_8,p:{x:33.25,y:31.8}},{t:this.instance_7,p:{x:587.25,y:116}},{t:this.instance_6,p:{x:280.55,y:427.1}},{t:this.instance_5,p:{x:37.55,y:338.2}},{t:this.instance_4,p:{x:259.25,y:-12.15}},{t:this.instance_3,p:{x:583.25,y:436.45}},{t:this.instance_2,p:{x:572.75,y:208.85}},{t:this.instance_1,p:{x:518.95,y:160.15}}]},59).wait(1));

	// жук1
	this.instance_9 = new lib.Фрагментролика();
	this.instance_9.setTransform(709.15,58.75,0.288,0.288,-15.0024,0,0,24.9,-8.4);

	this.timeline.addTween(cjs.Tween.get(this.instance_9).to({regY:-8.6,scaleX:0.351,scaleY:0.3509,rotation:0,x:-40.95,y:426.95},59).wait(1));

	this._renderFirstFrame();

}).prototype = p = new lib.AnMovieClip();
p.nominalBounds = new cjs.Rectangle(226.6,184.5,525.3,311.7);
// library properties:
lib.properties = {
	id: 'F7159E2B222A0046B2DDC1A5782D3000',
	width: 640,
	height: 480,
	fps: 30,
	color: "#FFFFFF",
	opacity: 1.00,
	manifest: [
		{src:"sounds/zvukletjaschegozhuka.mp3?1758691339968", id:"zvukletjaschegozhuka"},
		{src:"sounds/zyk1.mp3?1758691339968", id:"zyk1"}
	],
	preloads: []
};



// bootstrap callback support:

(lib.Stage = function(canvas) {
	createjs.Stage.call(this, canvas);
}).prototype = p = new createjs.Stage();

p.setAutoPlay = function(autoPlay) {
	this.tickEnabled = autoPlay;
}
p.play = function() { this.tickEnabled = true; this.getChildAt(0).gotoAndPlay(this.getTimelinePosition()) }
p.stop = function(ms) { if(ms) this.seek(ms); this.tickEnabled = false; }
p.seek = function(ms) { this.tickEnabled = true; this.getChildAt(0).gotoAndStop(lib.properties.fps * ms / 1000); }
p.getDuration = function() { return this.getChildAt(0).totalFrames / lib.properties.fps * 1000; }

p.getTimelinePosition = function() { return this.getChildAt(0).currentFrame / lib.properties.fps * 1000; }

an.bootcompsLoaded = an.bootcompsLoaded || [];
if(!an.bootstrapListeners) {
	an.bootstrapListeners=[];
}

an.bootstrapCallback=function(fnCallback) {
	an.bootstrapListeners.push(fnCallback);
	if(an.bootcompsLoaded.length > 0) {
		for(var i=0; i<an.bootcompsLoaded.length; ++i) {
			fnCallback(an.bootcompsLoaded[i]);
		}
	}
};

an.compositions = an.compositions || {};
an.compositions['F7159E2B222A0046B2DDC1A5782D3000'] = {
	getStage: function() { return exportRoot.stage; },
	getLibrary: function() { return lib; },
	getSpriteSheet: function() { return ss; },
	getImages: function() { return img; }
};

an.compositionLoaded = function(id) {
	an.bootcompsLoaded.push(id);
	for(var j=0; j<an.bootstrapListeners.length; j++) {
		an.bootstrapListeners[j](id);
	}
}

an.getComposition = function(id) {
	return an.compositions[id];
}


an.makeResponsive = function(isResp, respDim, isScale, scaleType, domContainers) {		
	var lastW, lastH, lastS=1;		
	window.addEventListener('resize', resizeCanvas);		
	resizeCanvas();		
	function resizeCanvas() {			
		var w = lib.properties.width, h = lib.properties.height;			
		var iw = window.innerWidth, ih=window.innerHeight;			
		var pRatio = window.devicePixelRatio || 1, xRatio=iw/w, yRatio=ih/h, sRatio=1;			
		if(isResp) {                
			if((respDim=='width'&&lastW==iw) || (respDim=='height'&&lastH==ih)) {                    
				sRatio = lastS;                
			}				
			else if(!isScale) {					
				if(iw<w || ih<h)						
					sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==1) {					
				sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==2) {					
				sRatio = Math.max(xRatio, yRatio);				
			}			
		}
		domContainers[0].width = w * pRatio * sRatio;			
		domContainers[0].height = h * pRatio * sRatio;
		domContainers.forEach(function(container) {				
			container.style.width = w * sRatio + 'px';				
			container.style.height = h * sRatio + 'px';			
		});
		stage.scaleX = pRatio*sRatio;			
		stage.scaleY = pRatio*sRatio;
		lastW = iw; lastH = ih; lastS = sRatio;            
		stage.tickOnUpdate = false;            
		stage.update();            
		stage.tickOnUpdate = true;		
	}
}
an.handleSoundStreamOnTick = function(event) {
	if(!event.paused){
		var stageChild = stage.getChildAt(0);
		if(!stageChild.paused || stageChild.ignorePause){
			stageChild.syncStreamSounds();
		}
	}
}
an.handleFilterCache = function(event) {
	if(!event.paused){
		var target = event.target;
		if(target){
			if(target.filterCacheList){
				for(var index = 0; index < target.filterCacheList.length ; index++){
					var cacheInst = target.filterCacheList[index];
					if((cacheInst.startFrame <= target.currentFrame) && (target.currentFrame <= cacheInst.endFrame)){
						cacheInst.instance.cache(cacheInst.x, cacheInst.y, cacheInst.w, cacheInst.h);
					}
				}
			}
		}
	}
}


})(createjs = createjs||{}, AdobeAn = AdobeAn||{});
var createjs, AdobeAn;