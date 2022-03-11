function canvasChart(dataIN) {
    console.log(dataIN)
    var dataINJSON = []
    dataINJSON = JSON.parse(dataIN)
    console.log(dataINJSON)
    var data_Expenditure = [];
    var data_Income = [];
    var data_Net = [];
    /*for (var i = 0; i < 5; i++) {
        data_Expenditure.push(
            {
                label: dataINJSON[i].Month,
                y: dataINJSON[i].Expenditure
            }
        )
    }*/
    dataINJSON.forEach(elem => data_Expenditure.push({ label: elem.Month, y: elem.Expenditure }))
    dataINJSON.forEach(elem => data_Income.push({ label: elem.Month, y: elem.Income }))
    dataINJSON.forEach(elem => data_Net.push({ label: elem.Month, y: elem.Net }))

    console.log(data_Expenditure)
    var chart = new CanvasJS.Chart("chartContainer", {
        theme: "light2", //gets rid of y-axis
        
        animationEnabled: true,  //change to false
        title: {
            text: "2020 Summary",
            fontSize: 20
        },
        data: [
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Expenditure",
                showInLegend: true,
                color: "#e27076",
                fillOpacity: .8,
                dataPoints: data_Expenditure
            },
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Income",
                showInLegend: true,
                color: "#00876c",
                fillOpacity: .8,
                dataPoints: data_Income
            },
            {
                // Change type to "doughnut", "line", "splineArea", etc.
                type: "column",
                name: "Net",
                showInLegend: true,
                color: "#0479cc",
                fillOpacity: .8,
                dataPoints: data_Net
            }
        ],
        /** Set axisY properties here*/
        axisY: {
            prefix: "$",
            margin:20
        },
        axisX: {
            margin:20
        }    
    });
    
    chart.render();
}