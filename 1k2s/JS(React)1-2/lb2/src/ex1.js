import React, { Component } from "react"


class Clock extends Component {
  constructor(props) {
    super(props);
    this.state = { date: new Date(),
    timezoneOffset: '-4:00' };
  }

  componentDidMount() { 
    this.timerID = setInterval(() => this.tick(), 1000);
  }
  componentWillUnmount() {
    clearInterval(this.timerID);
  }
  tick() {
    this.setState({
      date: new Date()
  });
  }

  time(date, offset) {
    let parts = offset.split(':');
    let hours = parseInt(parts[0]);
    let min = parseInt(parts[1]);

    let nDate = new Date(date.getTime() + hours * 1000 + min * 1000);

    return nDate.toLocaleTimeString('en-US', {hours12 : false});
  }

  

  render() {
    const { timezoneOffset} = this.state;
    const { date } = this.state;

    return (
      <div>
        <h2>Time: {this.time(date, timezoneOffset)}</h2>
      </div>
    );
  }
}

export default Clock;