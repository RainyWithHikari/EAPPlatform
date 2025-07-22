function GetRateLineChart(id, rateData, dateData, name, color) {
    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom, 'dark');
    var option;

    option = {
        title: {
            top: '3%',
            text: name,
            left: 'left',
            textStyle: {
                color: 'white',
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            },
            extraCssText: 'width:auto;height:auto'
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            left: 'right',
            top: '3%',
            textStyle: {
                color: "#fff"
            }
        },
        grid: {
            left: '15%',
            top: '18%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: dateData,
            axisLine: {
                lineStyle: {
                    color: "#fff",
                }
            },
            axisLabel: {
                rotate: 25,
            }
        },
        yAxis: [
            {
                type: 'value',
                name: 'Rate',
                position: 'left',
                min: 0,
                max: function (value) { return Math.ceil(value.max * 1.1) },
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
                    formatter: '{value} %',
                }
            }],
        series: [
            {
                name: 'Rate',
                type: 'line',
                data: rateData,
                areaStyle: {
                    color: color,
                },
                smooth: true,
                label: {
                    show: true,
                    rotate: 25,
                    fontSize: 15,
                    position: 'top',
                    align: 'left',
                    color: 'white',
                    formatter: function (params) {
                        return parseFloat(params.value).toFixed(1) + '%'
                    }
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

function GetSeriesBarChart(id, legend, seriesData, dateData, name) {
    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom, 'dark');
    var option;
    option = {
        title: {
            top: '3%',
            text: name,
            left: 'left',
            textStyle: {
                color: 'white',
            }
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            },
            extraCssText: 'width:auto;height:auto'
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            data: legend,
            left: 'right',
            orient: 'vertical', // 设置图例垂直排列
            /*top: '3%',*/
            textStyle: {
                color: "#fff"
            }
        },
        grid: {
            left: '12%',
            top: '23%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: dateData,
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
                name: 'tray',
                position: 'left',
                min: 0,
                max: function (value) { return Math.ceil(value.max) },
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
        series: seriesData
    };
    option && myChart.setOption(option);
    myChart.resize();

}

function GetCTScatterChart(id, legend, seriesData, name) {
    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom, 'dark');
    var option;
    option = {
        title: {
            top: '3%',
            text: name,
            left: 'left',
            textStyle: {
                color: 'white',
            }
        },
        tooltip: {
            formatter: function (params) {
                return params.marker + params.value[2] + "<br />" + "ReelId: " + params.value[3] + "<br />" + "PN: " + params.value[4] + "<br />" + "CT: " + params.value[1] + "<br />" + "CreateTime: " + params.value[0];
            }
        },
        axisPointer: {
            link: { xAxisIndex: 'all' },
            label: {
                backgroundColor: '#777'
            }
        },
        legend: {
            data: legend,
            left: 'right',
            top: '3%',
            textStyle: {
                color: "#fff"
            }
        },
        dataZoom: [
            {
                type: 'inside'
            }
            , {
                type: 'slider',
                showDataShadow: false,
                top: '93%',
                height: 10,
                handleIcon: 'M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z',
                handleSize: 10,
                handleStyle: {
                    color: 'white',
                    shadowBlur: 3,
                    shadowColor: 'rgba(0, 0, 0, 0.6)',
                    shadowOffsetX: 2,
                    shadowOffsetY: 2
                },
            }
        ],
        grid: {
            left: '12%',
            top: '23%'
        },
        xAxis: {
            type: 'time',
            axisTick: {
                alignWithLabel: true
            },
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
                name: 'second',
                position: 'left',
                min: 0,
                //max: function (value) { return Math.ceil(value.max) },
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
        series: seriesData
    };
    option && myChart.setOption(option);
    myChart.resize();

}

function GetSingleBarChart(id, dict, codes, values, name) {
    var chartDom = document.getElementById(id);
    $(window).on('resize', function () {
        //屏幕大小自适应，重置容器高宽
        chartDom.style.width = chartDom.getBoundingClientRect().width;
        chartDom.style.height = chartDom.getBoundingClientRect().height;
        myChart.resize();
    });
    chartDom.style.width = chartDom.getBoundingClientRect().width;
    chartDom.style.height = chartDom.getBoundingClientRect().height;

    var myChart = echarts.init(chartDom, 'dark');
    var option;

    option = {
        title: {
            top: '3%',
            text: name,
            left: 'left',
            textStyle: {
                color: 'white',
            }
        },

        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'shadow'
            },
            extraCssText: 'width:auto;height:auto',

            formatter: function (param) {

                var codetext = '';
                //int i
                for (var i = 0; i < dict.length; i++) {
                    if (dict[i].ALARMCODE == param[0].name) codetext = dict[i].ALARMTEXT + '<br/>' + param[0].data + '次';
                }
                return codetext;
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
            top: '3%',
            textStyle: {
                color: "#fff"
            }
        },
        grid: {


            //height: '60%'
        },
        xAxis: {
            type: 'category',
            axisTick: {
                alignWithLabel: true
            },
            data: codes,
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
                name: 'Times',
                position: 'left',

                min: 0,
                max: function (value) { return Math.ceil(value.max) },
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
                name: 'Times',
                type: 'bar',
                barHeight: '100%',

                emphasis: {
                    focus: 'series'
                },
                data: values,
                //itemStyle: {
                //    color: 'rgba(238, 232, 170, 1)',
                //},
                label: {
                    show: true,
                    //rotate: 25,
                    //position: 'center',
                    //align: 'left',
                    fontSize: 14,
                    color: 'white',
                    fontWeight: 'bold',
                    textStyle: {
                        textShadowColor: 'rgba(0, 0, 0, 0.5)', // 阴影颜色
                        textShadowBlur: 5 // 阴影模糊大小
                    },
                    //formatter: function (params) {
                    //    var value = parseFloat(params.value).toFixed(0)
                    //    if (value > 0) {
                    //        return parseFloat(params.value).toFixed(0);
                    //    } else {
                    //        return '';
                    //    }


                    //}
                    //formatter: function (params) {

                    //    return parseFloat(params.value).toFixed(0);

                    //}
                }

            }

        ]
    };
    option && myChart.setOption(option);
    myChart.resize();
}

function getValue(arr, field) {
    const values = new Array();
    for (const item of arr) {
        values.push(item[field]);
    }
    return values;
}

function getDistinctValue(arr, field) {
    const values = new Set();
    for (const item of arr) {
        values.add(item[field]);
    }
    return Array.from(values);
}
function getDistinctValues(arr, fields) {
    const values = new Set();
    for (const item of arr) {
        const groupValue = fields.map(field => item[field]).join(',');
        values.add(groupValue);
    }
    //return Array.from(values);
    return Array.from(values).map(groupValue => groupValue.split(','));
}

//找到数组中所有filed字段等于value的元素
function findElements(arr, fields, values) {
    return arr.filter(element =>
        fields.every((field, index) => element[field] === values[index])
    );
}