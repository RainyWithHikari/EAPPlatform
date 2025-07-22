import { getLatestAlarmRate } from '../dashboard/accbox_v3.js'

function getoutputchart(id, trendoutput, trenddates, target) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var option;

    option = {
        title: {
            text: window.eqid+'-Output Trends',
            textStyle: {
                color: "#fff",
                fontSize: 11
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        grid: {

            top: '20%',
            height: '60%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true,
                
            },
            data: trenddates,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            axisLabel: {
                rotate:25
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Output',
                position: 'left',
                min: function (value) { return 0 },
                max: function (value) { return Math.ceil(value.max + value.max * 0.2) },
                splitLine: {     //网格线
                    show: true,
                    lineStyle: {
                        color: '#22376d'
                    }
                },
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                }
            }],
        series: [
            {
                name: 'Output',
                type: 'bar',

                data: trendoutput,
                label: {
                    show: true,
                    position: 'top',
                    color: 'white'
                },
                itemStyle: {
                    color: '#6699CC',
                },
                markLine: {
                    silent: true,
                    itemStyle: {
                        normal: {
                            color: '#FA8565',

                        }
                    },
                    data: [{
                        yAxis: target,
                        label: {
                            color: 'white',
                            formatter: function (params) {
                                return parseFloat(params.value).toFixed(0) + '%'
                            }
                        }
                    }]
                }
            }

        ]
    };
    option && myChart.setOption(option);
    myChart.resize();

}
function getfpychart(id, trendfpy, trenddates) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);
    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var option;

    option = {
        title: {
            text: window.eqid+ '-Yield Trends',
            textStyle: {
                color: "#fff",
                fontSize: 11
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },

        grid: {
            height: '60%'

        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: trenddates,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            
            axisLabel: {
                rotate: 25
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Yield(%)',
                position: 'left',
                min: function (value) {
                    if (value.min == 0) return 0;
                    else {
                        //return Math.floor(value.min*0.5)
                        return 0
                    }
                },
                max: function (value) { return Math.ceil(value.max) },
                splitLine: {     //网格线
                    show: true,
                    lineStyle: {
                        color: '#22376d'
                    }
                },
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    },
                    axisLabel: {
                        formatter: '{value} %'
                    }
                }
            }],
        series: [
            {
                name: 'Yield',
                type: 'line',
                areaStyle: {
                    color: '#339933',
                },
                smooth: true,
                data: trendfpy,
                label: {
                    show: true,
                    rotate: 25, // 设置文本旋转角度为 45 度
                    position: 'top',
                    color: 'white',
                    formatter: '{c}'
                    //    function (params) {
                    //    console.log(params)
                    //    if (params.value != '100.00') {
                    //        return params.value + '%';
                    //    }
                    //    return '';
                    //}
                },
                lineStyle: {
                    color: 'yellow'
                }
            }


        ]
    };
    option && myChart.setOption(option);
    myChart.resize();

}
function getrunratechart(id, trenddates, trendrunrates) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);
    //console.log(trendrunrates);
    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var option;

    option = {
        title: {
            text: window.eqid+'-Cap. Utilization Trends',
            textStyle: {
                color: "#fff",
                fontSize: 11
                
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            data: ['runrate'],
            textStyle: {
                color: "#fff"
            }
        },
        grid: {
            height: '60%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: trenddates,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            axisLabel: {
                rotate:25
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Cap.(%)',
                position: 'left',
                min: function (value) { return 0 },
                max: function (value) { return Math.ceil(value.max + value.max * 0.1) },
                splitLine: {     //网格线
                    show: true,
                    lineStyle: {
                        color: '#22376d'
                    }
                },
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    },
                    axisLabel: {
                        formatter: '{value}'
                    }
                }

            }],

        series: [
            {
                name: 'Cap. Utilization',
                type: 'line',
                smooth: true,
                areaStyle: {
                    color: '#339933',
                },
                data: trendrunrates,
                label: {
                    show: true,
                    rotate: 25, // 设置文本旋转角度
                    position: 'top',
                    color: 'white',
                    formatter: '{c}'
                    //    function (params) {
                    //    if (params.value == Math.max.apply(null, trendrunrates) || params.value == Math.min.apply(null, trendrunrates)) {
                    //        return params.value+'%';
                    //    }
                    //    return '';
                    //}
                },
                lineStyle: {
                    color: 'yellow'
                }

            }

        ]
    };
    option && myChart.setOption(option);
    myChart.resize();

}

function drawGuage(id, data, maxdata, name) {

    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom);
    var option;
    option = {
        title: {
            text: name,
            top: 'bottom',
            left: 'center',
            textStyle: {
                color: 'white'
            }


        },
        series: [
            {
                type: 'gauge',
                radius: '70%',
                center: ['50%', '70%'],
                startAngle: 200,
                endAngle: -20,
                min: 0,
                max: maxdata,
                splitNumber: 10,
                itemStyle: {
                    color: '#FFAB91'
                },
                progress: {
                    show: true,
                    width: 30
                },
                pointer: {
                    show: false
                },
                axisLine: {
                    lineStyle: {
                        width: 30
                    }
                },
                axisTick: {
                    distance: -45,
                    splitNumber: 5,
                    lineStyle: {
                        width: 2,
                        color: '#999'
                    }
                },
                splitLine: {
                    distance: -52,
                    length: 14,
                    lineStyle: {
                        width: 3,
                        color: '#999'
                    }
                },
                axisLabel: {
                    distance: -10,
                    color: '#999',
                    fontSize: 10
                },
                anchor: {
                    show: false
                },
                title: {
                    show: false

                },
                detail: {
                    valueAnimation: true,
                    width: '70%',
                    lineHeight: 40,
                    borderRadius: 8,
                    offsetCenter: [0, '-15%'],
                    fontSize: 20,
                    fontWeight: 'bolder',
                    formatter: '{value}',
                    color: 'inherit'
                },
                data: [
                    {
                        value: data
                    }
                ]
            },
            {
                type: 'gauge',
                radius: '50%',
                center: ['50%', '70%'],
                startAngle: 200,
                endAngle: -20,
                min: 0,
                max: maxdata,
                itemStyle: {
                    color: '#FD7347'
                },
                progress: {
                    show: true,
                    width: 8
                },
                pointer: {
                    show: false
                },
                axisLine: {
                    show: false
                },
                axisTick: {
                    show: false
                },
                splitLine: {
                    show: false
                },
                axisLabel: {
                    show: false
                },
                detail: {
                    show: false
                },
                data: [
                    {
                        value: data
                    }
                ]
            }
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();

}


function gethistoryRate(id, rateData, weekData, time, target) {
    var chartDom = document.getElementById(id);
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = window.innerWidth * 0.195 + 'px';
        chartDom.style.height = window.innerHeight * 0.22 + 'px';
        myChart.resize();
    });
    chartDom.style.width = window.innerWidth * 0.195 + 'px'
    chartDom.style.height = window.innerHeight * 0.22 + 'px';


    var option;

    option = {
        title: {
            text: time+ "-Alarm Rate",
            textStyle: {
                color: "#fff",
                fontSize: 11
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            left: 'right',
            textStyle: {
                color: "#fff"
            }
        },
        grid: {
            top: '23%',
            height: '60%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: weekData,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            axisLabel: {
                rotate: 25
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Rate',
                position: 'left',
                min: 0,
                max: function (value) { return Math.ceil(value.max*1.6) },
                splitLine: {     //网格线
                    show: false,
                    lineStyle: {
                        color: '#22376d'
                    }
                },
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                },
                axisLabel: {
                    formatter: '{value} %'
                }
            }],
        series: [
            {
                name: 'Rate',
                type: 'line',
                data: rateData,
                label: {
                    show: true,
                    rotate: 25,
                    position: 'top',
                    align: 'left',
                    color: 'white',
                    formatter: function (params) {
                        return parseFloat(params.value).toFixed(1) +'%'
                    }
                },
                lineStyle: {
                    color: 'yellow'
                },
                markLine: {
                    silent: true,
                    itemStyle: {
                        normal: {
                            color: '#FA8565',
                        }
                    },
                    data: [{
                        yAxis: target,
                        label: {
                            color: 'white',
                            formatter: function (params) {
                                return parseFloat(params.value).toFixed(0) + '%'
                            }
                        }
                    }]
                }
            }
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();
    myChart.on('click', function (param) {
        //console.log(param.name);

        getLatestAlarmRate(param.name);
        window.week = param.name;
        //getStatusDetails(week);
    })
    return myChart;

}
function gethistoryTime(id, timeData, weekData, rundata,time) {
    var chartDom = document.getElementById(id);
    var chartParents = document.getElementById('history-alarm-rate');
    var myChart = echarts.init(chartDom);

    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = window.innerWidth * 0.195 + 'px';
        chartDom.style.height = window.innerHeight * 0.22 + 'px';
        myChart.resize();
    });
    chartDom.style.width = window.innerWidth * 0.195 + 'px'
    chartDom.style.height = window.innerHeight * 0.22 + 'px';
    //console.log(chartDom.style.width)
    //console.log(chartDom.style.height)

    var option;

    option = {
        title: {
            text: time+"-Alarm Time",
            textStyle: {
                color: "#fff",
                fontSize: 11
            }
        },

        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            //data: ['Time'],
            left: 'right',
            textStyle: {
                color: "#fff"
            }
        },
        grid: {
            left: '12%',
            top: '24%',
            height: '60%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: weekData,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            axisLabel: {
                rotate: 25
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Time(min)',
                position: 'left',
                min: 0,
                max: function (value) { return Math.ceil(value.max)*0.6 },
                splitLine: {     //网格线
                    show: false,
                    lineStyle: {
                        color: '#22376d'
                    }
                },
                axisLine: {
                    lineStyle: {
                        color: "#fff",
                    }
                },
                axisLabel: {
                    formatter: '{value}'
                }
            }],
        series: [
            
            {
                name: 'Alarm',
                type: 'bar',
                barHeight: '100%',
                stack: 'total',
                emphasis: {
                    focus: 'series'
                },
                data: timeData,
                itemStyle: {
                    color:'rgba(238, 232, 170, 1)',
                },
                label: {
                    show: true,
                    rotate: 25,
                    position: 'top',
                    align:'left',
                    color: 'white',
                    formatter: function (params) {
                        
                        return parseFloat(params.value).toFixed(1)

                    }
                }

            },
            {
                name: 'Run&Idle',
                type: 'bar',
                barHeight: '100%',
                stack: 'total',
                label: {
                    show: false
                },
                emphasis: {
                    focus: 'series'
                },
                data: rundata,
                itemStyle: {
                    color: 'rgba(255,255,255,0.3)',
                },
            },
            
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();
    myChart.on('click', function (param) {
        //console.log(param.name);

        getLatestAlarmRate(param.name);
        window.week = param.name;
        //getStatusDetails(week);
    })


}
function drawAlarmDetailByStation(id, data) {
    var chartDom = document.getElementById(id);
    let dataArr = []
    for (var i = 0; i < data.length; i++) {
        data[i].value = parseFloat(data[i].value)
        if (data[i].value != 0) {
            dataArr.push(data[i])
        }
    }
    function sortVal(a, b) {
        return b.value - a.value;
    }
    dataArr.sort(sortVal) //sort from big to small


    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom);
    var option;
    option = {
        title: {

            text: window.week + ' - Alarm Time',
            textStyle: {
                color: "#fff",
                fontSize: 15
            }
        },
        tooltip: {

        },
        legend: {
            top: 'bottom',
            textStyle: {
                color: "#fff",
                fontSize: 11
            }
        },
        toolbox: {
            show: true,
            feature: {
                mark: { show: true },
                dataView: { show: true, readOnly: false },
                restore: { show: true },
                saveAsImage: { show: true }
            }
        },
        series: [
            {

                name: 'Alarm time',
                type: 'pie',
                radius: [50, 105],
                center: ['50%', '50%'],
                roseType: 'area',
                itemStyle: {
                    borderRadius: 8
                },
                data: dataArr,
                label: {
                    formatter: '{name|{b}}\n{time|{c} min}',
                    color: 'white',
                    rich: {
                        time: {
                            fontSize: 10,
                            color: '#999'
                        }
                    }
                }
            }
        ]
    };
    option && myChart.setOption(option);
    myChart.resize();


}

function drawRader(id,data) {
    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {//
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom);
    var option;
    option = {
        title: {
            text: window.eqid,
            textStyle: {
                color: "#fff",
                fontSize: 15
            }
        },
        tooltip: {
            trigger: 'axis'
        },
        radar: 
            {
                indicator: [
                    { text: 'Yield', max: 100 },
                    { text: 'OEE', max: 100 },
                    { text: 'Cap. Utilization', max: 100 },
                    { text: 'Run Rate', max: 100 }
                ],
                center: ['50%', '50%'],
                radius: 60
            },
        series:
            {
                type: 'radar',
                tooltip: {
                    trigger: 'item'
                },
                areaStyle: {},
                data: [
                    {
                        value: data,
                        name: 'Behavior'
                    }
                ]
            }
    };

    option && myChart.setOption(option);
    myChart.resize();

}

export { getoutputchart, getfpychart, getrunratechart, drawGuage, gethistoryRate, gethistoryTime, drawAlarmDetailByStation, drawRader }