using System;

public class BresenhamData
{
    public BresenhamData()
    {
        m_Stepx = 0;
        m_Stepy = 0;
        m_E = 0;
        m_Deltax = 0;
        m_Deltay = 0;
        m_Origx = 0;
        m_Origy = 0;
        m_Destx = 0;
        m_Desty = 0;
    }
    public int m_Stepx;
    public int m_Stepy;
    public int m_E;
    public int m_Deltax;
    public int m_Deltay;
    public int m_Origx;
    public int m_Origy;
    public int m_Destx;
    public int m_Desty;
}

public class Bresenham
{
    /**
     *  \brief Initialize a TCOD_bresenham_data_t struct.
     *
     *  \param xFrom The starting x position.
     *  \param yFrom The starting y position.
     *  \param xTo The ending x position.
     *  \param yTo The ending y position.
     *  \param data Pointer to a TCOD_bresenham_data_t struct.
     *
     *  After calling this function you use TCOD_line_step_mt to iterate
     *  over the individual points on the line.
     */
    public static void LineInit(int xFrom, int yFrom, int xTo, int yTo, BresenhamData data)
    {
        data.m_Origx = xFrom;
        data.m_Origy = yFrom;
        data.m_Destx = xTo;
        data.m_Desty = yTo;
        data.m_Deltax = xTo - xFrom;
        data.m_Deltay = yTo - yFrom;
        if (data.m_Deltax > 0)
        {
            data.m_Stepx = 1;
        }
        else if (data.m_Deltax < 0)
        {
            data.m_Stepx = -1;
        }
        else
            data.m_Stepx = 0;
        if (data.m_Deltay > 0)
        {
            data.m_Stepy = 1;
        }
        else if (data.m_Deltay < 0)
        {
            data.m_Stepy = -1;
        }
        else
            data.m_Stepy = 0;
        if (data.m_Stepx * data.m_Deltax > data.m_Stepy * data.m_Deltay)
        {
            data.m_E = data.m_Stepx * data.m_Deltax;
            data.m_Deltax *= 2;
            data.m_Deltay *= 2;
        }
        else
        {
            data.m_E = data.m_Stepy * data.m_Deltay;
            data.m_Deltax *= 2;
            data.m_Deltay *= 2;
        }
    }
    /**
     *  \brief Get the next point on a line, returns true once the line has ended.
     *
     *  \param xCur An int pointer to fill with the next x position.
     *  \param yCur An int pointer to fill with the next y position.
     *  \param data Pointer to a initialized TCOD_bresenham_data_t struct.
     *  \return true after the ending point has been reached.
     *
     *  The starting point is excluded by this function.
     *  After the ending point is reached, the next call will return true.
     */
    public static bool LineStep(out int xCur, out int yCur, BresenhamData data)
    {
        xCur = 0;
        yCur = 0;
        if (data.m_Stepx * data.m_Deltax > data.m_Stepy * data.m_Deltay)
        {
            if (data.m_Origx == data.m_Destx) return true;
            data.m_Origx += data.m_Stepx;
            data.m_E -= data.m_Stepy * data.m_Deltay;
            if (data.m_E < 0)
            {
                data.m_Origy += data.m_Stepy;
                data.m_E += data.m_Stepx * data.m_Deltax;
            }
        }
        else
        {
            if (data.m_Origy == data.m_Desty) return true;
            data.m_Origy += data.m_Stepy;
            data.m_E -= data.m_Stepx * data.m_Deltax;
            if (data.m_E < 0)
            {
                data.m_Origx += data.m_Stepx;
                data.m_E += data.m_Stepy * data.m_Deltay;
            }
        }
        xCur = data.m_Origx;
        yCur = data.m_Origy;
        return false;
    }
    /**
     *  \brief Iterate over a line using a callback.
     *
     *  \param xo The origin x position.
     *  \param yo The origin y position.
     *  \param xd The destination x position.
     *  \param yd The destination y position.
     *  \param listener A TCOD_line_listener_t callback.
     *  \param data Pointer to a TCOD_bresenham_data_t struct.
     *  \return true if the line was completely exhausted by the callback.
     *
     *  \verbatim embed:rst:leading-asterisk
     *  .. deprecated:: 1.6.6
     *    The `data` parameter for this call is redundant, you should call
     *    :any:`TCOD_line` instead.
     *  \endverbatim
     */
    public static bool Line(int xo, int yo, int xd, int yd, Func<int, int, bool> listener, BresenhamData data)
    {
        LineInit(xo, yo, xd, yd, data);
        do
        {
            if (!listener(xo, yo)) return false;
        } while (!LineStep(out xo, out yo, data));
        return true;
    }
    /**
     *  \brief Iterate over a line using a callback.
     *
     *  \param xo The origin x position.
     *  \param yo The origin y position.
     *  \param xd The destination x position.
     *  \param yd The destination y position.
     *  \param listener A TCOD_line_listener_t callback.
     *  \return true if the line was completely exhausted by the callback.
     *
     *  \verbatim embed:rst:leading-asterisk
     *  .. versionchanged:: 1.6.6
     *    This function is now reentrant.
     *  \endverbatim
     */
    public static bool Line(int xo, int yo, int xd, int yd, Func<int, int, bool> listener)
    {
        BresenhamData data = new BresenhamData();
        return Line(xo, yo, xd, yd, listener, data);
    }
}
