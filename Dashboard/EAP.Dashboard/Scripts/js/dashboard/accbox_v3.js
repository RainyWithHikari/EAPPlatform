
import { OrbitControls } from '../../three/examples/jsm/controls/OrbitControls.js';
import { EffectComposer } from '../../three/examples/jsm/postprocessing/EffectComposer.js';

import { GUI } from '../../three/examples/jsm/libs/lil-gui.module.min.js';
import { Factory } from './factory.js';
import { positions } from '../config/position.js'
import { LabelLine } from '../effects/labelLine.js'
import { CSS2DRenderer, CSS2DObject } from '../../three/examples/jsm/renderers/CSS2DRenderer.js';

import { getoutputchart, getfpychart, getrunratechart, drawGuage, gethistoryRate, gethistoryTime, drawAlarmDetailByStation, drawRader } from '../utils/charts.js'
import { switchView,getDate, clearDoms, removeObj, getSelectEQPname, showStatusCount } from '../utils/tools.js'



window.week;
window.datetime = getDate();
window.eqid = "MAIN2";
window.clickObj;

var monthlyAlarmRateChart;

var camera, scene, renderer, controls, composer;
const css2Renderer = new CSS2DRenderer();

var canvas;
var width, height, ratio;
var gui = new GUI();
//var originalTargetPosition = THREE.Vector3(0,0,20)
var alarmObjs = [];
var runObjs = [];
var idleObjs = [];
var unknownObjs = ['MAIN1', 'MAIN3', 'MAIN8'];

var ambientLight, directionalLight;

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
getEQProfie(window.eqid);
GetHistoryAlarmRate();
GetRealTimeAlarm();


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
    camera.position.set(118, 20, 1.7);
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

    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(width, height);
    renderer.setClearAlpha(0.0);
    canvas.appendChild(renderer.domElement);

    //add light
    ambientLight = new THREE.AmbientLight('white', 0.5);
    directionalLight = new THREE.DirectionalLight(0xffffff, 0.5); // 平行光，颜色为白色，强度为1
    directionalLight.position.set(0.5, 1, 0); // 将平行光位置设为 (0, 1, 0)
    scene.add(ambientLight, directionalLight);

    //set controls
    controls = new OrbitControls(camera, renderer.domElement)
    controls.enableZoom = false;
    controls.minDistance = 0;
    controls.maxDistance = 150;
    controls.target.set(0, 15, 1.7);
    composer = new EffectComposer(renderer);

    const factory = new Factory(scene, camera, canvas, controls, composer);
    const clock = new THREE.Clock();

    //添加平面网格
    var gridHelper = new THREE.GridHelper(350, 10, 0x0F2D55, 0x0F2D55);
    gridHelper.name = 'gridHelper'
    scene.add(gridHelper);

    

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
        factory.start(clock.getDelta());
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
    start();
    datGui();
    //getColorofStatus(alarmObjs, runObjs, idleObjs);
    controls.addEventListener('change', function () {

        renderer.render(scene, camera)
        //console.log(camera.rotation)
    })

   
}
initFactory();
layui.use(['table'], function () {
    var table = layui.table
    GetStatus();
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
                tx:positionArr[currentPositionIndex].x,
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


const carousel = new bootstrap.Carousel('#carousel-alarm-time')
const carousel2 = new bootstrap.Carousel('#carousel-alarm-rate')
$(function () {
    $('.nav-overall').click(function () {
        window.datetime = getDate();
        switchView()
        getOverallData()
       
        carousel.cycle();
        carousel2.cycle();
    });
    if ($('.nav-overall').hasClass('active')) {
        window.datetime = getDate();


    }

    $('.nav-details').click(function () {
        window.datetime = getDate();
        let parentElement1 = document.getElementById('carousel-time');
        let parentElement2 = document.getElementById('carousel-rate');
        //console.log(parentElement.firstElementChild)
        
        let secondChild1 = parentElement1.querySelectorAll(':nth-child(2)');
        let secondChild2 = parentElement2.querySelectorAll(':nth-child(2)');
       
        secondChild1.forEach(function (child) {
            child.classList.remove('active');
        });
        secondChild2.forEach(function (child) {
            child.classList.remove('active');
        });
        parentElement1.firstElementChild.classList.add('active')
        parentElement2.firstElementChild.classList.add('active')
    
        carousel.pause();
        carousel2.pause();
        
        //console.log(carousel)
        //document.getElementById('carousel-alarm-time').setAttribute('data-bs-ride','false')
        switchView()

        getDetailData(window.eqid, window.datetime, 'Default')

        

    });


    //新增开线时间
    $("#addMfgTime").click(function () {
        layer.open({
            title: 'Add Manufaturing Time'
            , type: 2
            , btn: ['OK', 'Cancel']
            , content: 'ACCBoxDashboard/setWeek'
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
    $("#updateTagret").click(function () {
        layer.open({
            title: 'Set Target'
            , type: 2
            , id: 'SetTarget'
            , btn: ['OK', 'Cancel']
            , content: 'ACCBoxDashboard/setTarget'
            , area: ['30%', '30%']
            , success: function (layero, index) {
                //向layer页面传值，传值主要代码
                //   var body = layer.getChildFrame('body', index);
                var body = layer.getChildFrame('body', index);
                body.find("[id='line']").val('ACCBOX');
            }
            , yes: function (index) {
                var res = window["layui-layer-iframe" + index].callback();
                var data = JSON.parse(res);
                var target = data.Target;
                var eqptype = data.eqptype;
                //console.log(target);
                setTarget(target, eqptype);

                layer.close(index);
                //}

            }, btn2: function (index, layero) {
                layer.msg('取消操作');
            }

        });

    })
    $("#updateAlarmTarget").click(function () {
        console.log('click button')
        layer.open({
            title: 'Set Target'
            , type: 2
            , id: 'SetTarget'
            , btn: ['OK', 'Cancel']
            , content: 'ACCBoxDashboard/setTarget'
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
                var target = data.Target;
                var eqptype = data.eqptype;
                //console.log(target);
                setAlarmTarget(target, eqptype);

                layer.close(index);
                //}

            }, btn2: function (index, layero) {
                layer.msg('取消操作');
            }

        });

    })
    //导出报告
    $(".exportReport").click(function () {
        ExportAlarmDataByWeek(window.week);
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
});




window.onresize = function () {
    canvas = document.getElementById('map')

    width = canvas.clientWidth
    height = canvas.clientHeight
    ratio = width / height
   

    renderer.setSize(width, height);
    camera.aspect = ratio;
    camera.updateProjectionMatrix();

    gui.domElement.style.position = 'absolute';
    gui.domElement.style.left = canvas.offsetLeft * 1.1 + 'px';
    gui.domElement.style.top = canvas.offsetTop * 1.1 + 'px';

    monthlyAlarmRateChart.resize();
    GetStatus()
};

function getOverallData() {
    GetHistoryAlarmRate()
    GetRealTimeAlarm()
    GetStatus()
}
function getDetailData(eqp, datetime, selecttype) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "EQID": eqp,
            "datetime": datetime,
            "selecttype": selecttype// $("#datepicker").val(),
        },
        url: 'ACCBoxDashboard/GetStation',//ACCBoxDashboard/
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
                        , { field: 'ALARMTIME', title: 'Alarm Time', templet: '<div>{{ FormDate(d.ALARMTIME, "MM-dd HH:mm:ss") }}</div>', align: "center", width: '15%', unresize: true }
                        , { field: 'CA', title: 'Carrier ID', align: "center", width: '15%', unresize: true }
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

//获取历史机故率-by week
function GetHistoryAlarmRate() {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "eqpType":'ACCBOX'
        },
        url: 'ACCBoxDashboard/GetHistoryAlarmRate',//ACCBoxDashboard
        success: function (retdata) {
            var monthlyData = retdata.alarmMonArr;
            const months = monthlyData.map(obj => obj.time)
            const monthAlarms = monthlyData.map(obj => obj.alarmtimes)
            const monthMfg = monthlyData.map(obj => obj.mfgtimes - obj.alarmtimes)
            const monthRate = monthlyData.map(obj => (obj.alarmtimes / obj.mfgtimes) * 100)
            var target = retdata.target;
 
            monthlyAlarmRateChart = gethistoryRate('monthly-alarm-rate', monthRate, months, 'Monthly', target);
            gethistoryTime('monthly-alarm-time', monthAlarms, months, monthMfg, 'Monthly');

            var weeklyData = retdata.alarmWkArr
            const weeks = weeklyData.map(obj => obj.time)
            const weekAlarms = weeklyData.map(obj => obj.alarmtimes)
            const weekMfg = weeklyData.map(obj => obj.mfgtimes - obj.alarmtimes)
            const weekRate = weeklyData.map(obj => (obj.alarmtimes / obj.mfgtimes) * 100)
            gethistoryRate('history-alarm-rate', weekRate, weeks, 'Weekly', target);
            gethistoryTime('history-alarm-time', weekAlarms, weeks, weekMfg, 'Weekly');
            //console.log(weekRate)
            
            //console.log(months)
            var weekdata = retdata.weekdata;
            var totaltimes = retdata.totaltimes;
            var totalrates = retdata.totalrates;
            var mfgtime = retdata.totalmfg;

            window.week = weekdata[weekdata.length - 1]

            
            
            //gethistorychart('history-alarm-rate', totalrates, weekdata, totaltimes, maxtime, maxrate);
            getLatestAlarmRate(weekdata[weekdata.length - 1]);

        },
        error: function () {

        }
    });
}

//获取实时报警-all stations

function setStartEnd(starttime, endtime, idleduration,eqpType) {
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
        url: 'ACCBoxDashboard/ExportAlarmDataByWeek',//ACCBoxDashboard/
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

function ExportAlarmDataByStation(eqp, datetime) {
    layer.msg('报表下载中，请稍后！')
    $.ajax({
        type: 'post',
        data: {

            "EQID": eqp,
            "datetime": datetime,
        },
        url: 'ACCBoxDashboard/ExportAlarmDataByStation',//ACCBoxDashboard/
        xhrFields: {
            responseType: 'blob' // 将响应数据类型设置为blob
        },
        success: function (data, status, xhr) {

            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = eqp + "_" + datetime + "_" + "AlarmData.xlsx"; // 下载的文件名
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
function ExportYieldData(start,end) {

    layer.msg('报表下载中，请稍后！')
    $.ajax({
        type: 'post',
        data: {

            "start": start,
            "end":end,

        },
        url: 'ACCBoxDashboard/ExportYieldData',//ACCBoxDashboard/
        xhrFields: {
            responseType: 'blob' // 将响应数据类型设置为blob
        },
        success: function (data, status, xhr) {

            var blob = new Blob([data], { type: 'application/octet-stream' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "ACCBOX_"+start +"~"+ end +"_生产情况.zip"// 下载的文件名
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
            "eqpType":eqptype
        },
        url: 'ACCBoxDashboard/SetUPD',//ACCBoxDashboard/
        success: function (retdata) {


            location.reload();


        },
        error: function () {

        }
    });

}
function setAlarmTarget(target, eqptype) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "target": target,
            "eqpType": eqptype
        },
        url: 'ACCBoxDashboard/SetAlarmRateTarget',//ACCBoxDashboard/
        success: function (retdata) {


            location.reload();


        },
        error: function () {

        }
    });

}
function GetRealTimeAlarm() {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'ACCBoxDashboard/GetRealTimeAlarms',//ACCBoxDashboard/
        success: function (retdata) {
            const containerHeight = document.getElementById('real-time-alarm').clientHeight;
            
            var propertyName = 'EQID'; 
            var alarmdata = retdata.alarmdata.map(function (item) {
                return {
                    ...item,
                    [propertyName]: item[propertyName].toUpperCase(),
                    CA: item.ALARMTEXT.split(';').length < 3 ? "" : item.ALARMTEXT.split(';')[0],

                    ALARMTEXT: item.ALARMTEXT.split(';')[2] == null ? item.ALARMTEXT.split(';')[0] : item.ALARMTEXT.split(';')[2]
                };
            });;
            if (alarmdata) {
                layui.table.render({
                    elem: '#rt-alarm-table'

                    , cols: [[ //标题栏
                        { field: 'EQID', title: 'EQID', align: "center", width: '20%' }
                        , { field: 'ALARMTIME', title: 'Alarm Time', templet: '<div>{{ FormDate(d.ALARMTIME, "MM-dd HH:mm:ss") }}</div>', align: "center", width: '15%', unresize: true }
                        , { field: 'CA', title: 'Carrier ID', align: "center", width: '15%', unresize: true }
                        //, { field: 'SN', title: 'SN', width: '15%' }
                        , { field: 'ALARMTEXT', title: 'Alarm Text', align: "center", unresize: true }


                    ]]
                    , data: alarmdata
                    , size: 'lg'
                    , loading: true
                    , height: containerHeight
                    , limit: 8
                    , done: function (res, curr, count) {
                        
                        // 自定义渲染函数
                        var tableElem = this.elem.next('.layui-table-view'); // 获取表格元素
                        //console.log(tableElem)
                        tableElem.find('.layui-table-body tbody tr').each(function () {
                            var dataindex = $(this).attr('data-index');
                            //console.log(dataindex)
                            // 设置第一行数据的颜色为蓝色
                            if (dataindex == '0') {
                                $(this).css('color', 'rgba(255,102,255,1');
                            }
                        });
                    }


                });
            }

        },
        error: function () {

        }
    });

}

//获取具体某一周的AlarmRate -by station
function getLatestAlarmRate(selectweek) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "selectweek": selectweek,
            "eqpType":'ACCBOX'
        },
        url: 'ACCBoxDashboard/GetAlarmRate',//ACCBoxDashboard/
        success: function (retdata) {

            var alarmSpanByStation = retdata.alarmSpanByStation;
            drawAlarmDetailByStation('details-by-station', alarmSpanByStation)


            window.week = selectweek;




        },
        error: function () {

        }
    });
}

//获取某一station的具体alarmtime -by alarmcode
function getAlarmDetails(EQID, week) {
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {
            "EQID": EQID,
            "week": week
        },
        url: 'ACCBoxDashboard/GetAlarmDetails',//ACCBoxDashboard/
        success: function (retdata) {

            var alarmpie = retdata.alarmtotal.totalalarmpie;
            console.log(alarmpie);
            //getcodechart('codechart', alarmall, codelist, textlist, 'bar', week, EQID);
            //getcodepie('details-by-station', alarmpie, codelist, textlist, 'pie');
            drawAlarmDetailByStation('details-by-station', alarmpie)
        },
        error: function () {
            alert("error!");
        }
    });
}

function ChartssetInterval() {
    //每1分钟刷新一次界面
    setInterval(function () {
        GetStatus();
        getEQProfie(window.eqid)
        getColorofStatus()
        //render();
    }, 30000);
}

controls.addEventListener('change', function () {
    //console.log(camera)
    renderer.render(scene, camera); //执行渲染操作
});//监听鼠标、键盘事件





function GetStatus() {

    const containerHeight = document.getElementById('mfg-data').clientHeight;
    $.ajax({
        type: 'post',
        dataType: 'json',
        data: {

        },
        url: 'ACCBoxDashboard/GetStatus',//ACCBoxDashboard/
        success: function (retdata) {
            const data = retdata.eqpProfiles;
            const countByCategory = new Map();

            data.forEach(item => {
                const count = countByCategory.get(item.Status) || 0;
                countByCategory.set(item.Status, count + 1);
            });
            const table = document.getElementById('mfg-data-table')
            if (!table) {
                const tableNew = document.createElement('table')
                tableNew.id = 'mfg-data-table'
                const tableTitle = document.createElement('span')
                tableTitle.id = 'mfg-data-title'
                tableTitle.innerHTML = 'Real-time Data'
                document.getElementById('mfg-data').appendChild(tableTitle)
                document.getElementById('mfg-data').appendChild(tableNew)
            }
            showStatusCount(countByCategory);
            //console.log(retdata)
            if ($('.nav-overall').hasClass('active')) {
                layui.table.render({
                    elem: '#mfg-data-table'
                    , cols: [[ //标题栏
                        { field: 'EQID', title: 'EQID', align: "center", width: '20%', unresize: true }
                        , { field: 'Status', title: 'Status', align: "center", unresize: true }
                        , { field: 'output', title: 'Output', align: "center", unresize: true }
                        , { field: 'yield', title: 'Yield', align: "center", unresize: true }


                    ]]
                    , data: data
                    , size: 'sm'
                    , height: containerHeight
                    , loading: true
                    , limit: 14
                    , done: function (res, curr, count) {


                    }


                });


            }
           

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

function getEQProfie(EQID) {
    
        $.ajax({
            type: 'post',
            dataType: 'json',
            data: {
                "EQID": EQID,


            },
            url: 'ACCBoxDashboard/GetStationProfile',//ACCBoxDashboard/
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
                drawGuage('guage-data-output', parseFloat(rtoutput), parseFloat(target), 'Output')
                drawGuage('guage-data-yield', parseFloat(rtyield), 100, 'Yield(%)')
                drawGuage('guage-data-oee', OEEvalue.toFixed(2), 100, 'OEE(%)')

            }
            
        });

    
    
   
}



function generateAlarmNotification(infoArr) {


    const isExist = document.getElementById('map-alarm');
    if (isExist) {
        canvas.removeChild(isExist)
    }
    var alarmBox = document.createElement('div');
    alarmBox.id = 'map-alarm'
    alarmBox.style.position = 'absolute';
    alarmBox.style.bottom = '5px';
    alarmBox.style.right = '10px';
    alarmBox.style.padding = '10px';
    alarmBox.style.background = 'rgba(255, 0, 0, 0.5)';
    alarmBox.style.color = 'white';
    alarmBox.style.fontFamily = 'Arial';
    alarmBox.style.fontSize = '12px';
    alarmBox.style.zIndex = '2';
    var info = 'Alarm List' + '<br>';
    if (infoArr.length > 0) {

        for (var i = 0; i < infoArr.length; i++) {
            info += infoArr[i] + ' is alarming.' + '<br>'
        }

    }
    alarmBox.innerHTML = info


    canvas.appendChild(alarmBox)
}

getAlarmNotification();
function getAlarmNotification() {
    //generateAlarmNotification(alarmObjs)
    setInterval(function () {
        generateAlarmNotification(alarmObjs)
        getColorofStatus()
        //generateAlarmOutline(alarmObjs)

    }, 1500);
}
getSelectEQPname();

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
            var modelTag = scene.getObjectByName('tag_'+ alarmObjs[i])
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
            x: targetPosition.x +25,
            y: targetPosition.y +1,
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
        url: 'ACCBoxDashboard/GetStationProfile',//ACCBoxDashboard/
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

function getAlarmCone(){

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
            url: 'ACCBoxDashboard/GetStationAlarms',//ACCBoxDashboard/
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

function generateAlarmTags(obj,text) {

    
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
   
    new LabelLine(scene,obj.position,css2dObject.position)

}

    export { generateAlarmTags, removeMesh, getEQProfie, getSelectEQPname, getClickModel, getLatestAlarmRate, getDetailData, updateCameraPosition, createNamePlate, hideNamePlate, getColorofStatus, getAlarmCone, getAlarmPosition }
