import React, { Component } from 'react';

class JobMenu extends Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedProfession: null,
        };
    }

    handleProfessionChange = (profession) => {
        this.setState({ selectedProfession: profession });
    };

    renderMenuItems = () => {
        const { selectedProfession } = this.state;

        const professions = [
            { id: 1, name: 'Software Engineer', menuItems: ['Projects', 'Skills', 'Resume','Portfolio', 'Clients', 'Contact',`salary`] },
            { id: 2, name: 'Graphic Designer', menuItems: ['Projects', 'Skills', 'Resume','Portfolio', 'Clients', 'Contact',`salary`] },
            { id: 3, name: 'Manager', menuItems: ['Projects', 'Skills', 'Resume','Portfolio', 'Clients', 'Contact',`salary`] },
            { id: 4, name: 'Game Designer', menuItems: ['Projects', 'Skills', 'Resume','Portfolio', 'Clients', 'Contact',`salary`] },
            { id: 5, name: 'Game Developer', menuItems: ['Projects', 'Skills', 'Resume','Portfolio', 'Clients', 'Contact',`salary`] },
        ];

        const selectedProfessionData = professions.find((prof) => prof.id === selectedProfession);

        return (
            <div>
            <h3>{professions.find((prof) => prof.id === selectedProfession).name} Menu:</h3>
            <ul>
                {selectedProfessionData &&
                    selectedProfessionData.menuItems.map((item) => (
                        <li key={item}>{item}</li>
                    ))}
            </ul>
            </div>
        );
    };

    render() {
        const { selectedProfession } = this.state;

        return (
            <div>
                <h2>Choose a profession:</h2>
                <select onChange={(e) => this.handleProfessionChange(parseInt(e.target.value))}>
                    <option value={null}>Select a profession</option>
                    <option value={1}>Software Engineer</option>
                    <option value={2}>Graphic Designer</option>
                    <option value={3}>Manager</option>
                    <option value={4}>Game Designer</option>
                    <option value={5}>Game Developer</option>
                </select>

                {selectedProfession && (
                    <div>

                        {this.renderMenuItems()}
                    </div>
                )}
            </div>
        );
    }
}

export default JobMenu;