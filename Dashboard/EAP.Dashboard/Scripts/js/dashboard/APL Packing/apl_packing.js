
import { OrbitControls } from '../../../three/examples/jsm/controls/OrbitControls.js';
import { EffectComposer } from '../../../three/examples/jsm/postprocessing/EffectComposer.js';
import { RGBELoader } from '../../../three/examples/jsm/loaders/RGBELoader.js';
import { GUI } from '../../../three/examples/jsm/libs/lil-gui.module.min.js';
import { Factory } from './apl_packing_factory.js';
import { positions } from '../../config/position.js'//../config/position.js
import { LabelLine } from './effects/labelLine.js'//../effects/labelLine.js
import { CSS2DRenderer, CSS2DObject } from '../../../three/examples/jsm/renderers/CSS2DRenderer.js';

import { getoutputchart, getfpychart, getrunratechart, drawGuage, gethistoryRate, gethistoryTime, drawAlarmDetailByStation, drawRader, drawAlarmTimeBarChart, drawHourlyDownRateGauage, drawHourlyDownRateLine } from './utils/charts.js'
import { getDate, clearDoms, removeObj, getSelectEQPname, showStatusCount } from './utils/tools.js'



window.week;
window.datetime = getDate();
window.eqid = "MAIN2";
window.clickObj;
var alarmDataCache = [];



var camera, scene, renderer, controls, composer;
const css2Renderer = new CSS2DRenderer();

var canvas;
var width, height, ratio;
//var gui = new GUI();
//var originalTargetPosition = THREE.Vector3(0,0,20)
var alarmObjs = [];
var runObjs = [];
var idleObjs = [];
var unknownObjs = ['MAIN1', 'MAIN3', 'MAIN8'];

var ambientLight, directionalLight;
// 设备位置（相对图片的百分比）
const devicePositions = {
    'EQAPL00004': { left: 8, top: 40 },
    'EQATP00007': { left: 40, top: 55 },
    'EQAPL00005': { left: 32, top: 8 },
    'EQRSP00001': { left: 80, top: 70 }

}

var tween = new TWEEN.Tween();
// 创建一个变量来存储 Tween 动画的状态
let isAnimationPaused = false;
let pauseTime = 0;
let timeScale = 1;

const toastTrigger2 = document.getElementById('liveToastBtn2')
const toastLive2 = document.getElementById('liveToast2')
if (toastTrigger2) {
    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLive2)
    toastBootstrap._config.autohide = false;
    toastTrigger2.addEventListener('click', () => {
        //console.log(toastBootstrap)
        toastBootstrap.show()
    })
}


ChartssetInterval();
//getEQProfie(window.eqid);

GetStatusAndAlarm();
GetParamsStatus();
GetLineOEE();
GetHourlyDownTime(window.datetime);
GetDailyDownTime(window.datetime);


const initFactory = () => {

    canvas = document.getElementById('map')
    width = canvas.clientWidth
    height = canvas.clientHeight

    ratio = width / height

    //create scene
    scene = new THREE.Scene();

    //create camera
    camera = new THREE.PerspectiveCamera(45, ratio, 0.1, 2000);
    //camera.position.set(105, 25, 11);
    //camera.position.set(118, 20, -4);
    camera.position.set(1180, 10000, 10000);
    //camera.position.set(97, 30, 28);
    //camera.position.set(90, 27, 46);
    //camera.lookAt(90, 33, 101);
    camera.aspect = width / height;
    camera.updateProjectionMatrix();
    scene.add(camera)

    //create renderer
    if (canvas) {
        renderer = new THREE.WebGLRenderer({
            antialias: true,
            logarithmicDepthBuffer: true, //解决摩尔纹问题
        })
    }
    renderer.outputEncoding = THREE.SRGBEncoding;
    renderer.physicallyCorrectLights = true;

    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(width, height);
    renderer.setClearAlpha(0.0);
    canvas.appendChild(renderer.domElement);

    //add light
    //ambientLight = new THREE.AmbientLight('white', 1);
    //directionalLight = new THREE.DirectionalLight(0xffffff, 1); // 平行光，颜色为白色，强度为1
    //directionalLight.position.set(50, 100, 50); // 将平行光位置设为 (0, 1, 0)
    //scene.add(ambientLight, directionalLight);
    // 增加环境光强度
    ambientLight = new THREE.AmbientLight('white', 1); // 从1增加到2

    // 增加平行光强度和添加辅助光
    directionalLight = new THREE.DirectionalLight(0xffffff, 1); // 从1增加到2
    directionalLight.position.set(50, 100, 50);
    directionalLight.castShadow = true; // 如果需要阴影

    // 添加半球光作为补充
    const hemisphereLight = new THREE.HemisphereLight(0xffffff, 0x444444, 1);
    scene.add(ambientLight, directionalLight, hemisphereLight);
    //set controls
    controls = new OrbitControls(camera, renderer.domElement)
    controls.enableZoom = false;
    controls.minDistance = 0;
    controls.maxDistance = 150;
    controls.target.set(0, 15, 1.7);
    composer = new EffectComposer(renderer);


    const clock = new THREE.Clock();

    //添加平面网格
    var gridHelper = new THREE.GridHelper(350, 10, 0x0F2D55, 0x0F2D55);
    gridHelper.name = 'gridHelper'
    scene.add(gridHelper);

    // 环境贴图 HDR
    const basePath = window.location.pathname.split('/').slice(0, -1).join('/');
    const pmremGenerator = new THREE.PMREMGenerator(renderer);
    new RGBELoader()
        .setPath(`${basePath}/Content/HDR/`) // 替换成你的HDR文件路径
        .load('studio_small_09_1k.hdr', (texture) => {
            const envMap = pmremGenerator.fromEquirectangular(texture).texture;
            scene.environment = envMap;
            scene.background = envMap;
            texture.dispose();
            pmremGenerator.dispose();


        });

    new Factory(scene, camera, canvas, controls, composer);



    //const factory = new Factory(scene, camera, canvas, controls, composer);

    //var cameraFolder = gui.addFolder('Camera');

    //const cameraControls = {
    //    x: camera.position.x,
    //    y: camera.position.y,
    //    z: camera.position.z,
    //};

    // 在相机文件夹中添加控制器
    //cameraFolder.add(cameraControls, 'x', -1000, 1000).onChange(function (value) {
    //    camera.position.x = value;
    //}).listen();
    //cameraFolder.add(cameraControls, 'y', -1000, 1000).onChange(function (value) {
    //    camera.position.y = value;
    //}).listen();
    //cameraFolder.add(cameraControls, 'z', -1000, 1000).onChange(function (value) {
    //    camera.position.z = value;
    //}).listen();


    const start = () => {
        requestAnimationFrame(start)
        //factory.start(clock.getDelta());
        TWEEN.update();

        // 在每次渲染时更新 GUI 控件的值
        //cameraControls.x = camera.position.x;
        //cameraControls.y = camera.position.y;
        //cameraControls.z = camera.position.z;

        controls.update();
        renderer.render(scene, camera);
        css2Renderer.render(scene, camera)
        if (composer) {
            composer.render;

        }

    }
    //start();
    //datGui();
    //getColorofStatus(alarmObjs, runObjs, idleObjs);
    controls.addEventListener('change', function () {

        renderer.render(scene, camera)
        //console.log(camera.rotation)
    })


}
//initFactory();
layui.use(['table'], function () {
    var table = layui.table
    GetStatusAndAlarm();
});

function datGui() {
    // 设置对象的形式
    const parmas = {
        wireframe: false,//是否显示框线   --------- 设置开关
        bgColor: "#000000" // 场景颜色  --------- 设置颜色
    }


    gui.domElement.style.position = 'absolute';
    gui.domElement.style.left = canvas.offsetLeft * 1.1 + 'px';
    gui.domElement.style.top = canvas.offsetTop * 1.1 + 'px';

    var cameraControls = {
        resetCamera: function () {
            camera.position.set(118, 20, 1.7);
            controls.target.set(0, 15, 1.7);
            scene.traverse(obj => {

                if (obj.isMesh) {
                    obj.visible = true;

                }
                //obj.material.emissive.setHex(0x000000)
            })
            pauseTime = 0;
            timeScale = 1;
        }
    };

    //animation
    var cameraAnimations = {
        startAnimation: function () {
            console.log("start animation!")
            const positions = []
            const positionArr = []
            scene.traverse((object) => {
                if (object instanceof THREE.Mesh && (object.name.startsWith('MAIN') || object.name.startsWith('FCT'))) {
                    //console.log(object.position)

                    positionArr.push(object.position)


                }
            })
            positionArr.sort((a, b) => b.z - a.z);

            console.log(positionArr)
            let currentPositionIndex = 0;

            tween = new TWEEN.Tween({
                // 相机开始坐标
                x: camera.position.x,
                y: camera.position.y,
                z: camera.position.z,
                // 相机开始指向的目标观察点
                tx: camera.position.x,
                ty: camera.position.y,
                tz: camera.position.z,
            }).to({
                x: 45,//positionArr[currentPositionIndex].x + 30,
                y: 6.5,//y: positionArr[currentPositionIndex].y + 1.1,
                z: positionArr[currentPositionIndex].z,

                // 相机结束指向的目标观察点
                tx: positionArr[currentPositionIndex].x,
                ty: 6.5, //positionArr[currentPositionIndex].y,
                tz: positionArr[currentPositionIndex].z,
            }, 4000)
                .easing(TWEEN.Easing.Quadratic.InOut)
                .onComplete(() => {
                    console.log(currentPositionIndex)
                    currentPositionIndex++;
                    if (currentPositionIndex >= positionArr.length) {
                        currentPositionIndex = 0;
                    }
                    tween.to({
                        x: 45,
                        y: 6.5,//y: positionArr[currentPositionIndex].y + 1.1,
                        z: positionArr[currentPositionIndex].z,

                        // 相机结束指向的目标观察点
                        tx: positionArr[currentPositionIndex].x,
                        ty: 6.5, //positionArr[currentPositionIndex].y,
                        tz: positionArr[currentPositionIndex].z,
                    }, 4000).start();
                })
                .onUpdate(function (e) {
                    //小程序中使用e，H5中使用this，获取结束的位置信息
                    // 动态改变相机位置
                    camera.position.set(this.x, this.y, this.z);
                    // 模型中心点
                    controls.target.set(this.tx, this.ty, this.tz);
                    controls.update();//内部会执行.lookAt()
                })
                .start();
            isAnimationPaused = false;


        },
        pauseAnimation: function () {
            if (isAnimationPaused) {
                const currentTime = performance.now();
                const deltaTime = currentTime - pauseTime;
                timeScale = 4000 / deltaTime;
                tween.timeScale = timeScale;
                tween.start();
                isAnimationPaused = false;
            } else {
                tween.stop();
                pauseTime = performance.now();
                isAnimationPaused = true;
            }

            console.log(isAnimationPaused)
        }
    };

    // 设置颜色值，改变场景颜色
    gui.addColor(parmas, "bgColor").name("background color").
        onChange((val) => {
            scene.background = new THREE.Color(val);
        });
    gui.add(cameraControls, 'resetCamera').name('重置视角');
    gui.add(cameraAnimations, 'startAnimation').name('开始动画');
    gui.add(cameraAnimations, 'pauseAnimation').name('暂停/恢复动画');

    //console.log(gui)

}



//新增开线时间
$("#addMfgTime").click(function () {
    layer.open({
        title: 'Add Manufaturing Time'
        , type: 2
        , btn: ['OK', 'Cancel']
        , content: 'APLPacking/setWeek'
        , area: ['30%', '30%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            //   var body = layer.getChildFrame('body', index);
            var body = layer.getChildFrame('body', index);
            //var select = document.getElementById("eqp");
            //var options = select.options;
            //var index = select.selectedIndex;
            body.find("[id='line']").val('ACCBOX');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            //console.log(data);
            if (data.start >= data.end) {
                layer.msg('错误：开线时间大于结束时间！');
            } else {
                //console.log(data);
                // layer.msg(res);
                setStartEnd(data.start, data.end, data.idleduration, data.eqptype);
                layer.close(index);
            }

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });
});

//更新产能指标
$("#updateUPDTagret").click(function () {
    layer.open({
        title: 'Set Target'
        , type: 2
        , id: 'SetTarget'
        , btn: ['OK', 'Cancel']
        , content: 'APLPacking/setTarget'
        , area: ['30%', '30%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            //   var body = layer.getChildFrame('body', index);
            var body = layer.getChildFrame('body', index);
            body.find("[id='line']").val('APL Packing');
            body.find("[id='type']").val('UPD');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            var target = data.Target;
            var eqptype = data.eqptype;
            //var datatype = data.datatype;
            //console.log(target);
            setTarget(target, eqptype);

            layer.close(index);
            //}

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });

})
$("#updateHourlyAlarmTarget").click(function () {

    layer.open({
        title: 'Set Target'
        , type: 2
        , id: 'SetTarget'
        , btn: ['OK', 'Cancel']
        , content: 'APLPacking/setTarget'
        , area: ['30%', '30%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            //   var body = layer.getChildFrame('body', index);
            var body = layer.getChildFrame('body', index);
            //var select = document.getElementById("eqp");
            //var options = select.options;
            //var index = select.selectedIndex;
            body.find("[id='line']").val('APL Packing');
            body.find("[id='type']").val('HourlyDownTimeTarget');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            var target = data.Target;
            var eqptype = data.eqptype;
            var datatype = data.datatype
            //console.log(target);
            setAlarmTarget(target, eqptype, datatype);

            layer.close(index);
            //}

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });

})

$("#updateDailyAlarmTarget").click(function () {

    layer.open({
        title: 'Set Target'
        , type: 2
        , id: 'SetTarget'
        , btn: ['OK', 'Cancel']
        , content: 'APLPacking/setTarget'
        , area: ['30%', '30%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            //   var body = layer.getChildFrame('body', index);
            var body = layer.getChildFrame('body', index);
            //var select = document.getElementById("eqp");
            //var options = select.options;
            //var index = select.selectedIndex;
            body.find("[id='line']").val('APL Packing');
            body.find("[id='type']").val('DailyDownTimeTarget');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            var target = data.Target;
            var eqptype = data.eqptype;
            var datatype = data.datatype;
            //console.log(target);
            setAlarmTarget(target, eqptype, datatype);

            layer.close(index);
            //}

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });

})
//导出报告
$("#exportReports").click(function () {
    console.log('report')
    layer.open({
        title: '下载报表'
        , type: 2
        , btn: ['关闭']
        , content: 'APLPacking/DownloadReports'
        , area: ['55%', '60%']
        , success: function (layero, index) {
            ////向layer页面传值，传值主要代码
            var body = layer.getChildFrame('body', index);

            body.find("[id='dateFilter']").val($("#datepicker").val());
            body.find("[id='startdateFilter']").val($("#startdatepicker").val());
        }
        , yes: function (index) {
            layer.close(index);
        }

    });
   
});

$(".exportYieldReport").click(function () {
    //ExportYieldData(window.week);
    layer.open({
        title: 'Add Manufaturing Time'
        , type: 2
        , btn: ['OK', 'Cancel']
        , content: 'ACCBoxDashboard/setWeek'
        , area: ['30%', '40%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            //   var body = layer.getChildFrame('body', index);
            var body = layer.getChildFrame('body', index);
            //var select = document.getElementById("eqp");
            //var options = select.options;
            //var index = select.selectedIndex;
            body.find("[id='line']").val('ACC-BOX');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            if (data.start >= data.end) {
                layer.msg('错误：开始时间不可晚于结束时间！');
            } else {
                //console.log(data);
                // layer.msg(res);
                ExportYieldData(data.start, data.end);
                layer.close(index);
            }

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });
});

$("#datepicker").click(function () {

    layer.open({
        title: 'Manufaturing Calendar'
        , type: 2
        , btn: ['OK', 'Cancel']
        , content: 'ACCBoxDashboard/weekSelector'
        , area: ['40%', '50%']
        , success: function (layero, index) {
            //向layer页面传值，传值主要代码
            var body = layer.getChildFrame('body', index);

            body.find("[id='line']").val('ACC-BOX');
        }
        , yes: function (index) {
            var res = window["layui-layer-iframe" + index].callback();
            var data = JSON.parse(res);
            window.datetime = data.datetimePicker
            getDetailData(window.eqid, window.datetime, 'Default')

            layer.close(index);

        }, btn2: function (index, layero) {
            layer.msg('取消操作');
        }

    });
});





window.onresize = function () {
    canvas = document.getElementById('map')

    width = canvas.clientWidth
    height = canvas.clientHeight
    ratio = width / height


    //renderer.setSize(width, height);
    //camera.aspect = ratio;
    //camera.updateProjectionMatrix();

    //gui.domElement.style.position = 'absolute';
    //gui.domElement.style.left = canvas.offsetLeft * 1.1 + 'px';
    //gui.domElement.style.top = canvas.offsetTop * 1.1 + 'px';

    //monthlyAlarmRateChart.resize();
    GetStatus()


};

// 监听窗口缩放
$(window).on('resize', function () {
    const containerHeight = document.getElementById('real-time-alarm').clientHeight;
    const containerWidth = document.getElementById('real-time-alarm').clientWidth;
    layui.table.reload('rt-alarm-table', {
        height: containerHeight * 0.96,
        width: containerWidth
    });
});
function getDetailData(eqp, datetime, selecttype) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "EQID": eqp,
            "datetime": datetime,
            "selecttype": selecttype// $("#datepicker").val(),
        },
        url: 'APLPacking/GetStation',//ACCBoxDashboard/
        success: function (retdata) {
            if ($('.nav-details').hasClass('active')) {
                const data = retdata.alarmdata
                for (var i = 0; i < data.length; i++) {
                    data[i].EQID = window.eqid
                }
                let splitdata = data.map(item => {
                    return {
                        EQID: item.EQID,
                        ALARMTIME: item.ALARMTIME,
                        CA: item.ALARMTEXT.split(';').length < 3 ? "" : item.ALARMTEXT.split(';')[0],

                        ALARMTEXT: item.ALARMTEXT.split(';')[2] == null ? item.ALARMTEXT.split(';')[0] : item.ALARMTEXT.split(';')[2]
                    };

                })


                layui.table.render({
                    elem: '#rt-alarm-table'

                    , cols: [[ //标题栏
                        { field: 'EQID', title: 'EQID', align: "center", width: '20%', unresize: true }
                        , { field: 'ALARMTIME', title: 'Alarm Time', templet: '<div>{{ FormDate(d.ALARMTIME, "MM-dd HH:mm:ss") }}</div>', align: "center", width: '25%', unresize: true }
                        //, { field: 'CA', title: 'Carrier ID', align: "center", width: '15%', unresize: true }
                        , { field: 'ALARMTEXT', title: 'Alarm Text', align: "center", unresize: true }


                    ]]
                    , data: splitdata
                    , size: 'sm'
                    , loading: true
                    , limit: 7
                    , done: function (res, curr, count) {


                    }


                });

                var trendoutput = retdata.trenddata.outputs;

                var trendfpy = retdata.trenddata.fpys;

                var target = retdata.target;
                var trendoee = [];

                for (var i = 0; i < trendoutput.length; i++) {

                    trendoee[i] = ((parseFloat(trendoutput[i]) / target) * 100).toFixed(2)

                }

                var trendoutputsdates = retdata.trenddata.outputdates;
                var trendfpysdates = retdata.trenddata.fpysdates;

                getoutputchart('mfg-data', trendoutput, trendoutputsdates, target);
                getfpychart('history-alarm-time', trendfpy, trendfpysdates);
                getrunratechart('history-alarm-rate', trendoutputsdates, trendoee);
            }


            //getdatechart('datechart', chartdata);
        }
    });
}


function setStartEnd(starttime, endtime, idleduration, eqpType) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "starttime": starttime,
            "endtime": endtime,
            "idleduration": idleduration,
            "eqpType": eqpType
        },
        url: 'ACCBoxDashboard/SetStartEnd',//ACCBoxDashboard/
        success: function (retdata) {
            var eqdata = retdata.eqdata;
            var alarmrates = retdata.alarmrates;
            var starttime = retdata.starttime;
            var endtime = retdata.endtime;
            var alarmtimes = retdata.alarmtimes;
            var duration = starttime + "~" + endtime;
            var maxtime = retdata.maxtime;
            var maxrate = retdata.maxrate;
            //gettrendchart('trendchart', alarmrates, eqdata, alarmtimes, duration, maxtime, maxrate);
        },
        error: function () {

        }
    });

}
function ExportAlarmDataByWeek(week) {
    layer.msg('报表下载中，请稍后！')
    $.ajax({
        type: 'post',
        data: {

            "week": week

        },
        url: 'APLPacking/ExportAlarmDataByWeek',//ACCBoxDashboard/
        xhrFields: {
            responseType: 'blob' // 将响应数据类型设置为blob
        },
        success: function (data, status, xhr) {

            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ACCBOX_" + week + "_" + "AlarmData.xlsx"; // 下载的文件名
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            $('.spinner-border').hide();
        },
        error: function (err) {
            console.log(err)
        }
    });

}

function ExportYieldData(start, end) {

    layer.msg('报表下载中，请稍后！')
    $.ajax({
        type: 'post',
        data: {

            "start": start,
            "end": end,

        },
        url: 'APLPacking/ExportYieldData',//ACCBoxDashboard/
        xhrFields: {
            responseType: 'blob' // 将响应数据类型设置为blob
        },
        success: function (data, status, xhr) {

            var blob = new Blob([data], { type: 'application/octet-stream' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ACCBOX_" + start + "~" + end + "_生产情况.zip"// 下载的文件名
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            $('.spinner-border').hide();
        },
        error: function (err) {
            console.log(err)
        }
    });

}
function setTarget(target, eqptype) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "target": target,
            "eqpType": eqptype
        },
        url: 'APLPacking/SetUPD',//ACCBoxDashboard/
        success: function (retdata) {


            location.reload();


        },
        error: function () {

        }
    });

}
function setAlarmTarget(target, eqptype, datatype) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "target": target,
            "eqpType": eqptype,
            "dataType": datatype
        },
        url: 'APLPacking/SetDownTimeTarget',//ACCBoxDashboard/
        success: function (retdata) {
            location.reload();
        },
        error: function () {

        }
    });

}
function GetRealTimeAlarm(callback) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'APLPacking/GetRealTimeAlarms',//ACCBoxDashboard/
        success: function (retdata) {

            const containerHeight = document.getElementById('real-time-alarm').clientHeight;
            const containerWidth = document.getElementById('real-time-alarm').clientWidth;

            var propertyName = 'EQID';
            var alarmdata = retdata.alarmdata.map(function (item) {
                return {
                    ...item,
                    [propertyName]: item[propertyName].toUpperCase(),
                    CA: item.ALARMTEXT.split(';').length < 3 ? "" : item.ALARMTEXT.split(';')[0],

                    ALARMTEXT: item.ALARMTEXT.split(';')[2] == null ? item.ALARMTEXT.split(';')[0] : item.ALARMTEXT.split(';')[2]
                };
            });;

            alarmDataCache = alarmdata
            renderAlarmTable(containerHeight, containerWidth, alarmDataCache)
            if (callback) callback(); // 执行回调

        },
        error: function () {

        }
    });

}
function showAlarmBadge(eqid, eqName, alarmText) {
    const container = document.getElementById('alarm-container');

    // 已存在则不重复添加
    if (document.getElementById(`alarm-msg-${eqid}`)) return;

    const msg = document.createElement('div');
    msg.id = `alarm-msg-${eqid}`;
    msg.className = 'alarm-msg';
    msg.innerHTML = `<strong>${eqName}</strong>：${alarmText}`;

    container.appendChild(msg);
}
function removeAlarmBadge(eqid) {
    const msg = document.getElementById(`alarm-msg-${eqid}`);
    if (msg) msg.remove();
}

function renderAlarmTable(containerHeight,containerWidth,alarmdata) {
    if (alarmdata) {
        layui.table.render({
            elem: '#rt-alarm-table',
            height: containerHeight * 0.96,
            width: containerWidth,
            cols: [[
                { field: 'EQName', title: 'Station', align: "center", width: '25%', templet: '<div style="word-break: break-all;">{{d.EQName}}</div>' },
                { field: 'ALARMTIME', title: 'Alarm Time', templet: '<div>{{ FormDate(d.ALARMTIME, "MM-dd HH:mm:ss") }}</div>', align: "center", width: "25%", unresize: true },
                { field: 'ALARMTEXT', title: 'Alarm Text', align: "center", unresize: true }
            ]],
            data: alarmdata,
            size: 'sm',
            page: false,
            scroll: false,
            loading: true,
            //toolbar: '#alarmTableToolbar', // 引用外部模板
            defaultToolbar: [], // 关闭默认工具栏
            limit: alarmdata.length,
            layFilter: 'rt-alarm-table' ,
            done: function (res, curr, count) {
                var tableElem = this.elem.next('.layui-table-view');
                // 禁止横向滚动条
                tableElem.find('.layui-table-body').css({
                    'overflow-x': 'hidden',
                    'overflow-y': 'auto',
                    'background': '#1E1E2F'
                });

                // 表体区域滚动
                tableElem.find('.layui-table-body').css({
                    'overflow-y': 'auto'
                });

                // 增加每行之间的间隙
                tableElem.find('.layui-table-body tbody tr').css({
                    'margin-bottom': '6px',
                    'border-spacing': '0',
                    'background': '#1E1E2F' // 深色背景更协调
                });

                // 给单元格加内边距
                tableElem.find('.layui-table-cell').css({
                    'padding': '6px 4px',
                    'white-space': 'normal', // 文字换行
                    'word-break': 'break-word'
                });

            }
        });

        // 表头整体点击监听
        $(document).on('click', '.layui-table-header', function () {
            console.log('刷新触发');
            renderAlarmTable(
                document.getElementById('real-time-alarm').clientHeight,
                document.getElementById('real-time-alarm').clientWidth,
                alarmDataCache
            );
        });
    }
}

//获取具体某一周的AlarmRate -by station
function getLatestAlarmRate(selectweek) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "selectweek": selectweek,
            "eqpType": 'ACCBOX'
        },
        url: 'APLPacking/GetAlarmRate',//ACCBoxDashboard/
        success: function (retdata) {

            var alarmSpanByStation = retdata.alarmSpanByStation;
            drawAlarmDetailByStation('details-by-station', alarmSpanByStation)


            window.week = selectweek;




        },
        error: function () {

        }
    });
}

function ChartssetInterval() {
    //每1分钟刷新一次界面
    setInterval(function () {
        GetStatusAndAlarm();
        GetLineOEE();
        //GetRealTimeAlarm();
        GetParamsStatus();
        // 记录刷新时间
       
        //getEQProfie(window.eqid)
        //getColorofStatus()
        //render();
    }, 60000);
}





function GetStatusAndAlarm() {
    GetRealTimeAlarm(function () {
        GetStatus();
    });
}
function GetStatus() {

    //const containerHeight = document.getElementById('mfg-data').clientHeight;
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'APLPacking/GetStatus',//ACCBoxDashboard/
        success: function (retdata) {
            var now = new Date();
            var timeStr = now.toLocaleString();  // 本地时间格式
            document.getElementById('lastUpdateTime').innerText = 'Last Update：' + timeStr;

            const data = retdata.eqpProfiles;
            const countByCategory = new Map();

            data.forEach(item => {
                if (item.Status == 'Offline') item.Status = 'Down'
                const count = countByCategory.get(item.Status) || 0;
                countByCategory.set(item.Status, count + 1);

                // 设备处于Alarm时，打印报警信息
                if (item.Status === 'Alarm') {
                    const alarmInfo = alarmDataCache.find(a => a.EQID === item.EQID.toUpperCase());
                    if (alarmInfo) {
                        console.log(`设备 ${item.EQID} 报警：${alarmInfo.ALARMTEXT}`);
                        if (alarmInfo) {
                            showAlarmBadge(item.EQID, item.EQName, alarmInfo.ALARMTEXT);
                        }
                    } 
                } else {
                    console.log(`消除设备 ${item.EQID} 报警`);
                    removeAlarmBadge(item.EQID);

                }
            });

            showStatusCount(countByCategory);
            renderMfgData(data)
            // 初始渲染
            renderDevices(data);

            var statusdata = retdata.eqpProfiles;
            for (var i = 0; i < statusdata.length; i++) {
                removeObj(alarmObjs, statusdata[i].EQID)
                removeObj(runObjs, statusdata[i].EQID)
                removeObj(idleObjs, statusdata[i].EQID)


                if (statusdata[i].Status == 'Alarm') {

                    alarmObjs.push(statusdata[i].EQID)

                }
                else if (statusdata[i].Status == 'Run') {

                    runObjs.push(statusdata[i].EQID)


                }
                else if (statusdata[i].Status == 'Idle') {

                    idleObjs.push(statusdata[i].EQID)

                }




            }

        },
        error: function () {

        }
    });

}
function renderMfgData(data) {
    const container = document.getElementById('mfg-data-container');
    container.innerHTML = '';

    data.forEach(item => {
        if (item.Status == 'Down') item.Status = 'Offline'
        const statusColor = getStatusColor(item.Status);

        const card = document.createElement('div');
        card.style.cssText = `
            background: #163b5d;
            color: #fff;
            padding: 8px;
            border-radius: 6px;
            box-shadow: 0 0 4px rgba(0,0,0,0.3);
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            font-size: 0.9rem;
            min-width: 150px;
            box-sizing: border-box;
        `;

        card.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 4px;">
                <div style="font-weight: bold; color: #00d5ff; font-size: 18px;">${item.EQName}</div>
                <div style="color: ${statusColor}; font-weight: bold;">${item.Status}</div>
            </div>
            <div style="display: flex; justify-content: space-between;">
                <div>Output: ${item.output ?? '-'}</div>
                <div>Yield: ${item.yield ?? '-'}%</div>
            </div>
        `;

        container.appendChild(card);
    });
}

function renderDevices(deviceList) {
    const layer = document.getElementById('device-layer');
    layer.innerHTML = ''; // 清空

    const width = layer.clientWidth;
    const height = layer.clientHeight;
    const statusColors = {
        Run: 'green',
        Idle: '#F9A825',
        Alarm: 'red',
        Down: 'gray',
        Offline: 'darkgray'
    };


    deviceList.forEach(device => {
        const pos = devicePositions[device.EQID];
        if (!pos) return;

        const div = document.createElement('div');
        //div.className = 'device';
        div.className = `device ${device.Status.toLowerCase()}`;
        div.style.position = 'absolute'
        div.style.left = (width * pos.left / 100) + 'px';
        div.style.top = (height * pos.top / 100) + 'px';
        div.style.backgroundColor = statusColors[device.Status] || 'gray';

        div.innerHTML = `
            <div class="eqname">${device.EQName}</div>
            <div class="eqid">${device.EQID}</div>
        `;

        div.onclick = function () {
            filterAlarmByEqid(device.EQID);
        };

        layer.appendChild(div);
    });
}

function filterAlarmByEqid(eqid) {
    const filtered = alarmDataCache.filter(a => a.EQID === eqid.toUpperCase());
    renderAlarmTable(
        document.getElementById('real-time-alarm').clientHeight,
        document.getElementById('real-time-alarm').clientWidth,
        filtered
    );
}



function getStatusColor(status) {
    switch (status) {
        case 'Run': return '#00ff00';
        case 'Idle': return '#ffcc00';
        case 'Alarm': return '#ff4d4d';
        case 'Down': return '#666';
        case 'Offline': return '#666';
        default: return '#ccc';
    }
}

function GetParamsStatus() {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'APLPacking/GetParamsStatus',//ACCBoxDashboard/
        success: function (retdata) {
            //const containerHeight = document.getElementById('details-by-station').clientHeight;
            //console.log(retdata)
            if (retdata) {
                renderParamTable(retdata)

            }

        },
        error: function () {

        }
    });

}
function renderParamTable(data) {
    const tbody = document.getElementById('param-body');
    tbody.innerHTML = '';

    const groups = {};
    data.forEach(item => {
        if (!groups[item.EQName]) groups[item.EQName] = [];
        groups[item.EQName].push(item);
    });

    Object.keys(groups).forEach(eqName => {
        const params = groups[eqName];
        params.forEach((item, index) => {
            const value = parseFloat(item.Value);
            const lower = item.LCL ?? -Infinity;
            const upper = item.UCL ?? Infinity;
            const warnLower = item.WarnLCL ?? -Infinity;
            const warnUpper = item.WarnUCL ?? Infinity;

            let statusColor = 'green';
            let fontColor = '#fff';

            if (value < lower || value > upper) {
                statusColor = 'red'; // 超限
                fontColor = '#FF5555';
            } else if (value < warnLower || value > warnUpper) {
                statusColor = 'yellow'; // 警戒
                fontColor = '#FFA500';
            }

            const tr = document.createElement('tr');
            tr.style.borderBottom = '1px solid #005599';

            let eqNameCell = '';
            if (index === 0) {
                eqNameCell = `<td rowspan="${params.length}" style="padding: 6px; border: 1px solid #005599; font-weight: bold; background: #002244;">${eqName}</td>`;
            }

            tr.innerHTML = `
                ${eqNameCell}
                <td class="param-name" style="padding: 6px; border: 1px solid #005599; cursor: pointer;"
                    data-eqid="${item.EQID}"
                    data-eqname="${eqName}"
                    data-name="${item.Name}"
                    data-value="${item.Value}"
                    data-lcl="${item.LCL ?? ''}"
                    data-ucl="${item.UCL ?? ''}"
                    data-warnlcl="${item.WarnLCL ?? ''}"
                    data-warnucl="${item.WarnUCL ?? ''}">
                    ${item.Name}
                </td>
                <td style="padding: 6px; border: 1px solid #005599; color:${fontColor};">${item.Value}</td>
                <td style="padding: 6px; border: 1px solid #005599; text-align: center;">
                    <div style="width: 13px; height: 13px; border-radius: 50%; background: ${statusColor}; display: inline-block;"></div>
                </td>
            `;
            tbody.appendChild(tr);
        });
    });

    document.querySelectorAll('.param-name').forEach(cell => {
        cell.addEventListener('click', function () {
            const eqid = this.getAttribute('data-eqid');
            const eqName = this.getAttribute('data-eqname');
            const name = this.getAttribute('data-name');
            const value = this.getAttribute('data-value');
            const lcl = this.getAttribute('data-lcl');
            const ucl = this.getAttribute('data-ucl');
            const warnlcl = this.getAttribute('data-warnlcl');
            const warnucl = this.getAttribute('data-warnucl');

            layui.use('layer', function () {
                const layer = layui.layer;

                layer.open({
                    type: 1,
                    title: '参数详情 - ' + name,
                    area: ['500px', '450px'],
                    content: `
                        <div style="padding: 20px;">
                            <form class="layui-form" id="paramDetailForm">
                                <div class="layui-form-item">
                                    <label class="layui-form-label">设备名称</label>
                                    <div class="layui-form-mid">${eqName}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">参数名称</label>
                                    <div class="layui-form-mid">${name}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">当前值</label>
                                    <div class="layui-form-mid">${value}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">控制下限(LCL)</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="lcl" value="${lcl}" placeholder="请输入控制下限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">控制上限(UCL)</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="ucl" value="${ucl}" placeholder="请输入控制上限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">警戒下限</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="warnlcl" value="${warnlcl}" placeholder="请输入警戒下限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">警戒上限</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="warnucl" value="${warnucl}" placeholder="请输入警戒上限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <div class="layui-input-block">
                                        <button class="layui-btn" lay-submit lay-filter="paramSubmit">保存修改</button>
                                        <button type="button" class="layui-btn layui-btn-primary" id="cancelBtn">取消</button>
                                    </div>
                                </div>
                                <input type="hidden" name="eqid" value="${eqid}">
                            </form>
                        </div>
                    `,
                    success: function (layero, index) {
                        layui.form.render();

                        layui.form.on('submit(paramSubmit)', function (data) {
                            submitParamChanges(data.field, eqName, name, cell);
                            layer.close(index);
                            return false;
                        });

                        document.getElementById('cancelBtn').onclick = function () {
                            layer.close(index);
                        };
                    }
                });
            });
        });
    });
}


function renderParamTable_old(data) {
    const tbody = document.getElementById('param-body');
    tbody.innerHTML = '';

    // 按 EQName 分组
    const groups = {};
    data.forEach(item => {
        if (!groups[item.EQName]) {
            groups[item.EQName] = [];
        }
        groups[item.EQName].push(item);
    });

    Object.keys(groups).forEach(eqName => {
        const params = groups[eqName];
        params.forEach((item, index) => {
            const value = parseFloat(item.Value);
            const lower = item.LCL ?? -Infinity;
            const upper = item.UCL ?? Infinity;

            let statusColor = 'green'; // 正常
            if (value === lower || value === upper) statusColor = 'yellow'; // 临界
            if (value < lower || value > upper) statusColor = 'red'; // 超限

            const tr = document.createElement('tr');
            tr.style.borderBottom = '1px solid #005599';

            let eqNameCell = '';
            if (index === 0) {
                eqNameCell = `<td rowspan="${params.length}" style="padding: 6px; border: 1px solid #005599; font-weight: bold; background: #002244;">${eqName}</td>`;
            }
            // /* <td style="padding: 8px; border: 1px solid #005599;">${item.LCL ?? '--'} ~ ${item.UCL ?? '--'}</td>*/
            tr.innerHTML = `
                ${eqNameCell}
                <td class="param-name" style="padding: 6px; border: 1px solid #005599; cursor: pointer;"
                    data-eqid = "${item.EQID}"
                    data-eqname="${eqName}" 
                    data-name="${item.Name}" 
                    data-value="${item.Value}" 
                    data-lcl="${item.LCL ?? '--'}" 
                    data-ucl="${item.UCL ?? '--'}">
                    ${item.Name}
                </td>
                <td style="padding: 6px; border: 1px solid #005599;">${item.Value}</td>
              
                <td style="padding: 6px; border: 1px solid #005599; text-align: center;">
                    <div style="width: 13px; height: 13px; border-radius: 50%; background: ${statusColor}; display: inline-block;"></div>
                </td>
            `;
            tbody.appendChild(tr);
        });

    });
    // 添加点击事件监听器
    document.querySelectorAll('.param-name').forEach(cell => {
        cell.addEventListener('click', function () {
            const eqid = this.getAttribute('data-eqid');
            const eqName = this.getAttribute('data-eqname');
            const name = this.getAttribute('data-name');
            const value = this.getAttribute('data-value');
            let lcl = this.getAttribute('data-lcl');
            let ucl = this.getAttribute('data-ucl');

            // 转换 -- 为空白
            lcl = lcl === '--' ? '' : lcl;
            ucl = ucl === '--' ? '' : ucl;

            // 使用 Layui 的 layer 打开表单
            layui.use('layer', function () {
                const layer = layui.layer;

                layer.open({
                    type: 1,
                    title: '参数详情 - ' + name,
                    area: ['500px', '400px'],
                    content: `
                        <div style="padding: 20px;">
                            <form class="layui-form" id="paramDetailForm">
                                <div class="layui-form-item">
                                    <label class="layui-form-label">设备名称</label>
                                    <div class="layui-form-mid">${eqName}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">参数名称</label>
                                    <div class="layui-form-mid">${name}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">当前值</label>
                                    <div class="layui-form-mid">${value}</div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">控制下限(LCL)</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="lcl" value="${lcl}" placeholder="请输入控制下限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <label class="layui-form-label">控制上限(UCL)</label>
                                    <div class="layui-input-block">
                                        <input type="number" name="ucl" value="${ucl}" placeholder="请输入控制上限" class="layui-input">
                                    </div>
                                </div>
                                <div class="layui-form-item">
                                    <div class="layui-input-block">
                                        <button class="layui-btn" lay-submit lay-filter="paramSubmit">保存修改</button>
                                        <button type="button" class="layui-btn layui-btn-primary" id="cancelBtn">取消</button>
                                    </div>
                                </div>
                                <input type="hidden" name="eqid" value="${eqid}">
                            </form>
                        </div>
                    `,
                    success: function (layero, index) {
                        // 表单渲染
                        layui.form.render();

                        // 提交表单
                        layui.form.on('submit(paramSubmit)', function (data) {
                            //console.log(data.field)
                            submitParamChanges(data.field, eqName, name, cell);
                            layer.close(index);
                            return false;
                        });

                        // 取消按钮
                        document.getElementById('cancelBtn').onclick = function () {
                            layer.close(index);
                        };
                    }
                });
            });


        });
    });
}

// 提交参数修改到后端
function submitParamChanges(formData, eqName, paramName, cellElement) {
    const loadIndex = layui.layer.load(1);

    const postData = {
        eqid: formData.eqid,
        eqName: eqName,
        paramName: paramName,
        lcl: formData.lcl || null,
        ucl: formData.ucl || null,
        warnlcl: formData.warnlcl || null,
        warnucl: formData.warnucl || null
    };

    $.ajax({
        url: 'APLPacking/UpdateParamLimits',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(postData),
        success: function (response) {
            layui.layer.close(loadIndex);
            if (response.success) {

                // 更新前端 data-* 属性
                cellElement.setAttribute('data-lcl', formData.lcl || '');
                cellElement.setAttribute('data-ucl', formData.ucl || '');
                cellElement.setAttribute('data-warnlcl', formData.warnlcl || '');
                cellElement.setAttribute('data-warnucl', formData.warnucl || '');

                layui.layer.msg('修改成功', { icon: 1 });
            } else {
                layui.layer.msg(response.message || '修改失败', { icon: 2 });
            }
        },
        error: function (xhr) {
            layui.layer.close(loadIndex);
            layui.layer.msg('请求失败: ' + xhr.statusText, { icon: 2 });
        }
    });
}

function submitParamChanges_old(formData, eqName, paramName, cellElement) {
    // 显示加载中
    const loadIndex = layui.layer.load(1);

    // 准备提交数据
    const postData = {
        eqid: formData.eqid,
        eqName: eqName,
        paramName: paramName,
        lcl: formData.lcl || null,  // 如果为空则传 null
        ucl: formData.ucl || null
    };

    // 这里使用 jQuery 的 AJAX，你也可以用原生 fetch 或 axios
    $.ajax({
        url: 'APLPacking/UpdateParamLimits', // 替换为你的后端接口
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(postData),
        success: function (response) {
            layui.layer.close(loadIndex);
            if (response.success) {
                // 更新前端显示
                if (formData.lcl) {
                    cellElement.setAttribute('data-lcl', formData.lcl);
                } else {
                    cellElement.setAttribute('data-lcl', '--');
                }
                if (formData.ucl) {
                    cellElement.setAttribute('data-ucl', formData.ucl);
                } else {
                    cellElement.setAttribute('data-ucl', '--');
                }

                layui.layer.msg('修改成功', { icon: 1 });
            } else {
                layui.layer.msg(response.message || '修改失败', { icon: 2 });
            }
        },
        error: function (xhr) {
            layui.layer.close(loadIndex);
            layui.layer.msg('请求失败: ' + xhr.statusText, { icon: 2 });
        }
    });
}

function GetLineOEE() {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'APLPacking/GetLineOEE',//ACCBoxDashboard/
        success: function (retdata) {
            console.log("OEE = " + retdata.data)
            drawGuage('guage-data-oee', retdata.data.toFixed(2), 100, 'OEE' + ` (UPD = ${retdata.target})`)
        },
        error: function () {

        }
    });
}

function GetHourlyDownTime(datetime) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "datetime": datetime
        },
        url: 'APLPacking/GetHourlyDownTime',
        success: function (retdata) {
            //console.log(retdata)
            renderCharts(retdata, datetime)

        },
        error: function () {

        }
    });
}

function GetDailyDownTime(datetime) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "datetime": datetime
        },
        url: 'APLPacking/GetDailyDownTime',
        success: function (retdata) {
            console.log(retdata)
            drawAlarmTimeBarChart('alarm-time', retdata.downtimes, retdata.downtimetarget)

            //drawGuage('guage-data-oee', retdata.toFixed(2), 100, 'OEE(%)')
        },
        error: function () {

        }
    });
}

// 每小时刷新数据
function refreshHourlyData() {
    // 获取数据逻辑
    GetHourlyDownTime(window.datetime);

    // 每小时执行一次（3600000毫秒=1小时）
    setInterval(function () {
        //console.log('refresh')
        GetHourlyDownTime(window.datetime);
    }, 3600000);// 3600000
}


// 每天刷新数据
function refreshDailyData() {
    // 获取数据逻辑
    GetDailyDownTime(window.datetime);

    // 计算到下一个午夜的时间
    const now = new Date();
    const midnight = new Date(
        now.getFullYear(),
        now.getMonth(),
        now.getDate() + 1, // 明天
        0, 0, 0 // 午夜00:00:00
    );
    const timeUntilMidnight = midnight - now;

    // 设置定时器，先等待到午夜，然后每天执行
    setTimeout(() => {
        GetDailyDownTime(window.datetime);
        setInterval(() => { GetDailyDownTime(window.datetime) }, 86400000); // 86400000毫秒=1天
    }, timeUntilMidnight);
}


refreshHourlyData();
refreshDailyData();

function renderCharts(retdata,datetime) {
    const alarms = retdata.downtimes || [];
    const alarmtarget = retdata.downtimetarget || 0;

    if (alarms.length === 0) return;

    const lastData = alarms[alarms.length - 1]?.Minutes || 0;
    let maxData = Math.max(lastData, alarmtarget);
    if (maxData == 0) maxData = 100

    // ===== 仪表盘图表 =====
    //'gauge-hourly-downtime'
    drawHourlyDownRateGauage('gauge-hourly-downtime', lastData, maxData)
    // ===== 折线图 =====
    /*'line-hourly-downtime'*/
    drawHourlyDownRateLine('line-hourly-downtime', alarms, alarmtarget, datetime)

}

function getEQProfie(EQID) {

    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "EQID": EQID,


        },
        url: 'APLPacking/GetStationProfile',//ACCBoxDashboard/
        success: function (retdata) {
            const target = retdata.target;
            const profileData = retdata.rtdata;


            var rtoutput, rtyield;
            for (var i = 0; i < profileData.length; i++) {
                if (profileData[i].Name == 'output') {
                    rtoutput = parseFloat(profileData[i].Value);
                } else {
                    rtyield = parseFloat(profileData[i].Value);
                }

            }
            if (rtyield == 0) {
                rtyield = 100
            }

            var datenow = new Date().toLocaleString('chinese', { hour12: false });
            var dateDay = getDate() + " 09:00:00";

            var dateNight = getDate() + " 21:00:00";
            var OEEvalue;

            if (new Date(dateDay) <= new Date(datenow) && new Date(dateNight) >= new Date(datenow)) {
                var timespan = new Date(datenow).getTime() - new Date(dateDay).getTime()
                var totalspan = new Date(dateNight).getTime() - new Date(dateDay).getTime()

                var realtimeTarget = parseFloat(target) * (timespan / totalspan)
                if (realtimeTarget == 0) {
                    realtimeTarget = parseFloat(target)
                }

                OEEvalue = ((rtoutput * (rtyield / 100)) / realtimeTarget) * 100;

            } else {

                OEEvalue = ((rtoutput * (rtyield / 100)) / parseFloat(target)) * 100;

            }
            if (OEEvalue > 100) {
                console.log("OEE exceed 100: " + OEEvalue);
                OEEvalue = 100
            }
            //drawGuage('guage-data-output', parseFloat(rtoutput), parseFloat(target), 'Output')
            //drawGuage('guage-data-yield', parseFloat(rtyield), 100, 'Yield(%)')
            //drawGuage('guage-data-oee', OEEvalue.toFixed(2), 100, 'OEE(%)')

        }

    });




}




function getClickModel(clickObj, formerObj) {
    //console.log(clickObj)
    //console.log(formerObj)
    window.clickObj = clickObj;
    if (clickObj != null) {
        if (alarmObjs.includes(clickObj.name)) {
            clickObj.material.emissive.setHex(0x800080)
            clickObj.material.emissiveIntensity = 0.3
        } else if (idleObjs.includes(clickObj.name)) {
            clickObj.material.emissive.setHex(0xFFA500)
            clickObj.material.emissiveIntensity = 0.3

        } else {
            clickObj.material.emissive.setHex(0x00ff00)
            clickObj.material.emissiveIntensity = 0.3
        }
    }
    if (formerObj != null) {
        if (alarmObjs.includes(formerObj.name)) {
            formerObj.material.emissive.setHex(0xff0000)
            formerObj.material.emissiveIntensity = 0.3
        } else if (idleObjs.includes(formerObj.name)) {
            formerObj.material.emissive.setHex(0xFFFF00)
            formerObj.material.emissiveIntensity = 0.3
        } else {
            formerObj.material.emissive.setHex(0x000000)
            formerObj.material.emissiveIntensity = 0.3
        }
    }


}

function getColorofStatus() {

    if (alarmObjs.length > 0) {
        for (var i = 0; i < alarmObjs.length; i++) {
            var model = scene.getObjectByName(alarmObjs[i])
            var modelTag = scene.getObjectByName('tag_' + alarmObjs[i])
            model.material.emissive.setHex(0xff0000)
            //modelTag.material.emissive.setHex(0xff0000)
            model.material.emissiveIntensity = 0.3
            //modelTag.material.emissiveIntensity = 0.3

            var tagname = 'tag_' + alarmObjs[i]

            var modelTag = scene.getObjectByName(tagname)
            var colorUniform = modelTag.material.uniforms.color;
            var newColor = new THREE.Color(0xff0000);
            colorUniform.value.copy(newColor);
        }

    }
    if (runObjs.length > 0) {
        for (var i = 0; i < runObjs.length; i++) {
            var model = scene.getObjectByName(runObjs[i])
            model.material.emissive.setHex(0x000000)
            var tagname = 'tag_' + runObjs[i]

            var modelTag = scene.getObjectByName(tagname)
            var colorUniform = modelTag.material.uniforms.color;
            var newColor = new THREE.Color(0x00ff00); // 新的颜色为绿色
            colorUniform.value.copy(newColor);
            //console.log(modelTag.material.uniforms.color)

        }

    }
    if (idleObjs.length > 0) {
        for (var i = 0; i < idleObjs.length; i++) {
            var model = scene.getObjectByName(idleObjs[i])
            model.material.emissive.setHex(0xffff00)
            model.material.emissiveIntensity = 0.3
            var tagname = 'tag_' + idleObjs[i]

            var modelTag = scene.getObjectByName(tagname)
            var colorUniform = modelTag.material.uniforms.color;
            var newColor = new THREE.Color(0xffff00);
            colorUniform.value.copy(newColor);
        }

    }
    for (var i = 0; i < unknownObjs.length; i++) {
        //station with no data, set default as green
        var tagname = 'tag_' + unknownObjs[i]

        var modelTag = scene.getObjectByName(tagname)
        var colorUniform = modelTag.material.uniforms.color;
        var newColor = new THREE.Color(0x00ff00);
        colorUniform.value.copy(newColor);
    }

}



function updateCameraPosition(targetPosition) {
    controls.minDistance = 0;
    controls.maxPolarAngle = Math.PI / 1;
    controls.update();

    new TWEEN.Tween({
        // 相机开始坐标
        x: camera.position.x,
        y: camera.position.y,
        z: camera.position.z,
        // 相机开始指向的目标观察点
        tx: camera.position.x,
        ty: camera.position.y,
        tz: camera.position.z,
    })
        .to({
            // 相机结束坐标
            x: targetPosition.x + 25,
            y: targetPosition.y + 1,
            z: targetPosition.z,
            // 相机结束指向的目标观察点
            tx: targetPosition.x,
            ty: targetPosition.y,
            tz: targetPosition.z,
        }, 1000)
        .onUpdate(function (e) {
            //小程序中使用e，H5中使用this，获取结束的位置信息
            // 动态改变相机位置
            camera.position.set(this.x, this.y, this.z);
            // 模型中心点
            controls.target.set(this.tx, this.ty, this.tz);
            controls.update();//内部会执行.lookAt()
        })
        .start();
}

function createNamePlate(EQID) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "EQID": EQID,

        },
        url: 'APLPacking/GetStationProfile',//ACCBoxDashboard/
        success: function (retdata) {
            const target = retdata.target;
            const profileData = retdata.rtdata;
            const runrate = retdata.status.RunRate * 100;
            //const mtba = retdata.status.Mtba;

            var rtoutput, rtyield;
            for (var i = 0; i < profileData.length; i++) {
                if (profileData[i].Name == 'output') {
                    rtoutput = parseFloat(profileData[i].Value);
                } else {
                    rtyield = parseFloat(profileData[i].Value);
                }

            }
            if (rtyield == 0) {
                rtyield = 100
            }

            var datenow = new Date().toLocaleString('chinese', { hour12: false });
            var dateDay = getDate() + " 09:00:00";

            var dateNight = getDate() + " 21:00:00";
            var OEEvalue, capValue;

            if (new Date(dateDay) <= new Date(datenow) && new Date(dateNight) >= new Date(datenow)) {
                var timespan = new Date(datenow).getTime() - new Date(dateDay).getTime()
                var totalspan = new Date(dateNight).getTime() - new Date(dateDay).getTime()

                var realtimeTarget = parseFloat(target) * (timespan / totalspan)
                if (realtimeTarget == 0) {
                    realtimeTarget = parseFloat(target)
                }
                capValue = (rtoutput / realtimeTarget) * 100;
                OEEvalue = ((rtoutput * (rtyield / 100)) / realtimeTarget) * 100;

            } else {
                capValue = (rtoutput / parseFloat(target)) * 100;
                OEEvalue = ((rtoutput * (rtyield / 100)) / parseFloat(target)) * 100;

            }

            if (OEEvalue > 100) OEEvalue = 100;
            if (capValue > 100) capValue = 100;
            var raderArr = [parseFloat(rtyield).toFixed(2), OEEvalue.toFixed(2), capValue.toFixed(2), runrate.toFixed(2)];
            const nameplateDom = document.getElementById('nameplate')
            nameplateDom.style.zIndex = 4
            nameplateDom.removeAttribute('hidden')
            var content = document.getElementById('text')
            content.innerHTML = window.eqid
            clearDoms(content)

            drawRader('text', raderArr)


        }

    });


}

function hideNamePlate() {
    const nameplateDom = document.getElementById('nameplate')
    nameplateDom.hidden = true;
}

function getAlarmCone() {

    var alarmMeshs = [];
    if (alarmObjs != null) {
        alarmObjs.forEach(item => {
            var model = scene.getObjectByName(item)
            alarmMeshs.push(model)
        })

    }

    return alarmMeshs;
}

function getAlarmPosition() {
    var alarmPositions = []
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": window.eqid,
                "datetime": window.datetime
            },
            url: 'APLPacking/GetStationAlarms',//ACCBoxDashboard/
            success: function (retdata) {
                //console.log(retdata)
                const data = retdata.result;
                //console.log(data)
                if (data) {
                    data.forEach(item => {
                        var obj = { alarmtext: item.AlarmText, alarmtime: item.AlarmTime, positionID: item.PositionID }
                        //removeObj(alarmPositions, obj.alarmcode)

                        alarmPositions.push(obj)
                    })
                }

                //console.log(alarmPositions)
                //alarmPositions = [
                //    { alarmtext: 'test', alarmtime: 'test', positionID: 'MAIN7_P01' },
                //    { alarmtext: 'test', alarmtime: 'test', positionID: 'MAIN7_P02' },
                //    { alarmtext: 'test', alarmtime: 'test', positionID: 'MAIN7_P03' },


                //]
                //alarmPositions.push(obj)
                var resultArr = [];
                //console.log(alarmPositions.PositionID)
                //const result = positions.filter(obj => alarmPositions.includes(obj.positionID)).map(obj => ({ position: obj.position})) //&& obj.EQID == window.eqid
                const result = alarmPositions.filter(item1 => {

                    const matchItem = positions.find(item2 => item2.positionID === item1.positionID);
                    var alarmTimeStamp = parseInt(item1.alarmtime.match(/\d+/)[0]);
                    var alarmTimeStr = new Date(alarmTimeStamp).toLocaleTimeString();
                    var obj = matchItem ? { text: alarmTimeStr + '<br>' + item1.alarmtext, position: matchItem.position } : null;
                    if (obj) { resultArr.push(obj) };
                })
                //console.log(resultArr)
                resolve(resultArr);
            },
            error: function (error) {
                reject(error)
            }
        });
    })


}
function removeMesh(meshName) {
    var meshNameToRemove = meshName;
    var objectToRemove = [];

    // 遍历场景的子对象，查找匹配名称的网格
    scene.traverse(function (child) {
        if ((child instanceof THREE.Mesh || child instanceof THREE.Line || child.type == 'Object3D') && child.name === meshNameToRemove) {
            objectToRemove.push(child);
        }
    });

    // 如果找到了匹配的网格，则从场景中删除它
    if (objectToRemove !== null) {
        objectToRemove.forEach(item => {
            scene.remove(item);
        })
    }
}

function generateAlarmTags(obj, text) {


    css2Renderer.setSize(width, height);
    document.getElementById('map').appendChild(css2Renderer.domElement)

    var label = document.createElement('div');
    label.className = 'label';
    label.id = 'map-alarm-label'

    label.style.background = 'rgba(255, 0, 0, 0.5)';
    label.style.color = 'white';
    label.style.fontFamily = 'Arial';
    label.style.padding = '0.8%';
    label.style.fontSize = '14px';
    label.innerHTML = text;
    label.appendChild(document.createElement('br'));
    var button = document.createElement('button');
    button.classList.add('btn', 'btn-danger')
    button.innerHTML = 'Handle'
    //button.style.height = '25px'
    //button.style.fontSize = '11px'
    button.setAttribute('disabled', true);
    label.appendChild(button)

    var css2dObject = new CSS2DObject(label);


    var displayPosition = {
        x: obj.position.x,
        y: obj.position.y,
        z: obj.position.z,

    }
    var division = displayPosition.z - window.clickObj.position.z
    if (obj.position.y > 5) {
        displayPosition.y -= 3
    } else {
        displayPosition.y += 3
    }
    if (division < 0) {
        displayPosition.z -= 10
    } else {
        displayPosition.z += 10
    }
    css2dObject.position.copy(displayPosition);
    css2dObject.name = 'alarmLabel'

    scene.add(css2dObject);

    new LabelLine(scene, obj.position, css2dObject.position)

}

export { GetHourlyDownTime, generateAlarmTags, removeMesh, getEQProfie, getSelectEQPname, getClickModel, getLatestAlarmRate, getDetailData, updateCameraPosition, createNamePlate, hideNamePlate, getColorofStatus, getAlarmCone, getAlarmPosition }
