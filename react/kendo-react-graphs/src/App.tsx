import React from 'react';
import './App.css';
import axios from 'axios';
import {
  Chart,
  ChartSeries,
  ChartSeriesItem,
  ChartTitle,
  ChartLegend,
} from "@progress/kendo-react-charts";

interface Result {
    date: string,
    value: number
}
interface props {
    series: Result[]
}

export class App extends React.Component<any,props> {
    //private series: Result[];

    constructor(props: any) {
        super(props);
        this.state = {
            series: [
                {
                    date: "1 day",
                    value: 100
                }]
        }
    }



    componentDidMount(){
        axios.get<Result[]>("https://localhost:5001/User/GetRetention")
            .then(res => {
                //this.series = res.data;
                this.setState({
                    series: res.data
                })
            }).catch(error=> {
            console.log(error.response.data.text);
        })


    }



    render() {
        return(
            <div className="App">
                <Chart style={{ height: 350 }}>
                    <ChartTitle text="Rolling Retention 7 day" />
                    <ChartLegend position="top" orientation="horizontal" />

                    <ChartSeries>
                        {this.state.series.map((item, idx) => (
                            <ChartSeriesItem
                                key={idx}
                                type="column"
                                tooltip={{ visible: true }}
                                data={[item.value]}
                            />
                        ))}
                    </ChartSeries>
                </Chart>
            </div>
        );
    }
}
