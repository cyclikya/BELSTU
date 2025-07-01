import React, { useState } from 'react';



const Catalog = ({ products }) => {
    const [sortColumn, setSortColumn] = useState(null);
    const [sortOrder, setSortOrder] = useState('asc');

    const handleSort = (column) => {
        if (sortColumn === column) {
            setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
        } else {
            setSortColumn(column);
            setSortOrder('asc');
        }
    };

    const sortedProducts = products.sort((a, b) => {
        if (sortColumn === 'name') {
            return sortOrder === 'asc' ? a.name.localeCompare(b.name) : b.name.localeCompare(a.name);
        } else if (sortColumn === 'price') {
            return sortOrder === 'asc' ? a.price - b.price : b.price - a.price;
        } else {
            return a.id - b.id;
        }
    });

    const totalQuantity = products.reduce((total, product) => total + product.quantity, 0);
    const totalPrice = products.reduce((total, product) => total + product.price * product.quantity, 0);

    return (
        <div>
            <table>
                <thead>
                <tr>
                    <th>number of string</th>
                    <th onClick={() => handleSort('name')}>Name project</th>
                    <th onClick={() => handleSort('price')}>cost</th>
                    <th>count</th>
                </tr>
                </thead>
                <tbody>
                {sortedProducts.map((product, index) => (
                    <tr key={product.id} style={{ backgroundColor: product.quantity < 3 ? 'yellow' : 'white' }}>
                        <td>{index + 1}</td>
                        <td>{product.name}</td>
                        <td>{product.price}</td>
                        <td style={{ color: product.quantity === 0 ? 'red' : 'black' }}>{product.quantity}</td>
                    </tr>
                ))}
                </tbody>
            </table>
            <div>
                General count: {totalQuantity}
                <br />
                General cost: {totalPrice}
            </div>
        </div>
    );
};

export default Catalog;