import axios from 'axios';

//  const apiUrl = "http://localhost:5224";  // אם אתה לא רוצה להשתמש ב-HTTPS
const apiUrl = process.env.REACT_APP_API_URL; // אם אתה לא רוצה להשתמש ב-HTTPS

export default {
//   getTasks: async () => {
//     const result = await axios.get(`${apiUrl}/items`)    
//     return result.data;
//   },

  addTask: async (name) => {
    
    try {
        const result = await axios.post(`${apiUrl}/items`, {
            name: name,
            isComplete: false  // הוספת מצב ברירת מחדל (לא הושלם)
        });
        console.log('Task added successfully', result.data);
    } catch (error) {
        console.error('Error adding task:', error);
    }
},

getTasks: async () => {
  try {
      const response = await axios.get(`${apiUrl}/items`);
      console.log("✅ API response:", response.data); // 🔍 בדיקה
      return Array.isArray(response.data) ? response.data : []; // לוודא שזה מערך
  } catch (error) {
      console.error("❌ Error fetching tasks:", error);
      return []; // במקרה של שגיאה, מחזירים מערך ריק
  }
}
,
deleteTask: async (id) => {
  try {
    await axios.delete(`${apiUrl}/items/${id}`);
    return id; // מחזיר את ה-ID למחיקה בצד הלקוח
  } catch (error) {
    console.error("Error deleting task:", error);
  }
}
,


updateTask: async (id, name, isComplete) => {
  try {
    const updatedTask = {
      Name: name,  
      IsComplete: isComplete 
    };

    // שולח את הנתונים לשרת
    const result = await axios.put(`${apiUrl}/items/${id}`, updatedTask, {
      headers: {
        'Content-Type': 'application/json'  // חשוב שה-Content-Type יהיה json
      }
    });

    console.log('Task updated successfully:', result.data);
    return result.data;
  } catch (error) {
    // טיפול בשגיאות כולל הצגת פרטי השגיאה
    console.error('Error updating task:', error);

    if (error.response) {
      // אם יש תשובת שגיאה מהשרת (למשל 500)
      console.error('Error response:', error.response.data);
      console.error('Error status:', error.response.status);
    } else if (error.request) {
      // אם לא התקבלה תשובה מהשרת
      console.error('No response received:', error.request);
    } else {
      // אם הייתה בעיה כלשהי בהגדרת הבקשה
      console.error('Error message:', error.message);
    }

    // זורק את השגיאה כדי שניתן יהיה לתפוס אותה בצד הלקוח
    throw error; 
  }
}
,
setCompleted: async (id,name, isComplete) => {
  try {
    // שולח רק את השדה 'isComplete' ב-JSON
    const result = await axios.put(`${apiUrl}/items/${id}`, {
      name: name,  
  isComplete: isComplete 
    }, {
      headers: {
        'Content-Type': 'application/json'  // חשוב שה-Content-Type יהיה json
      }
    });

    console.log('Task updated successfully:', result.data);
  } catch (error) {
    console.error('Error updating task:', error);

    if (error.response) {
      console.error('Error response:', error.response.data);
      console.error('Error status:', error.response.status);
    } else if (error.request) {
      console.error('No response received:', error.request);
    } else {
      console.error('Error message:', error.message);
    }
  }
}


};