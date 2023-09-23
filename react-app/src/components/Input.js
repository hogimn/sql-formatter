import React, {useEffect, useState} from 'react';

function Input({onInputChange}) {
    // noinspection SqlDialectInspection,SqlNoDataSourceInspection
    const [inputValue, setInputValue] = useState(
        'SELECT supplier_name, city FROM suppliers\n' +
        'WHERE supplier_id > 500\n' +
        'ORDER BY supplier_name ASC, city DESC;');

    useEffect(() => {
        const handleChange = () => {
            onInputChange(inputValue);
        };

        let timeout;
        if (inputValue.trim()) {
            timeout = setTimeout(handleChange, 500);
        } else {
            onInputChange('');
        }

        return () => {
            clearTimeout(timeout);
        };
    }, [inputValue, onInputChange]);

    return (
        <section className="input">
            <textarea
                id="input"
                autoFocus
                wrap="off"
                value={inputValue}
                onChange={(e) => setInputValue(e.target.value)}
            />
        </section>
    );
}

export default Input;
