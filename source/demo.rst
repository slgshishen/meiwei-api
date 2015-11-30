语法范例
=============

.. contents:: Table of Contents

.. topic:: Topic Title

   Subsequent indented lines comprise
   the body of the topic, and are
   interpreted as body elements.

.. sidebar:: Sidebar Title
   :subtitle: Optional Sidebar Subtitle

   Subsequent indented lines comprise
   the body of the sidebar, and are
   interpreted as body elements.

标题
====

章
--

节
~~

小节
####

.. Strong Emphasis

This is **Strong Text**. HTML tag is strong.粗体

.. Italic, Emphasis

This is *Emphasis* Text.这个HTML使用em， 斜体

.. Interpreted Text

This is `Interpreted Text`. 注意，这个HTML一般用<cite>表示

.. Inline Literals

This is ``Inline Literals``. HTML tag is <tt>. 等宽字体.

我这里是一个 链接_.

.. _链接: http://www.9now.cn

.. This is a comment.

..
   This whole indented block
   is a comment.

   Still in the comment.

.. figure:: _static/images/logo.png
   :scale: 50 %
   :alt: map to buried treasure


.. literalinclude:: _code/example.js
   :language: javascript
   :linenos:

.. code:: python

  def my_function():
      "just a test"
      print 8/2

.. table::

   =====  =====
     A    not A
   =====  =====
   False  True
   True   False
   =====  =====

+----------------+-------------------------------------+
|     参数       | 说明                                |
+================+=====================================+
| HTTP请求方式   | POST                                |
+----------------+-------------------------------------+
| 返回数据格式   |  JSON                               |
+----------------+-------------------------------------+
| 测试环境       |  http://test2.9now.net/api/menu/dish|
+----------------+-------------------------------------+
| 正式环境       |  http://api.9now.cn/api/menu/dish   |
+----------------+-------------------------------------+

+------------------------+------------+----------+----------+
| Header row, column 1   | Header 2   | Header 3 | Header 4 |
| (header rows optional) |            |          |          |
+========================+============+==========+==========+
| body row 1, column 1   | column 2   | column 3 | column 4 |
+------------------------+------------+----------+----------+
| body row 2             | Cells may span columns.          |
+------------------------+------------+---------------------+
| body row 3             | Cells may  | - Table cells       |
+------------------------+ span rows. | - contain           |
| body row 4             |            | - body elements.    |
+------------------------+------------+---------------------+

.. csv-table:: Frozen Delights!
   :header: "Treat", "Quantity", "Description"
   :widths: 15, 10, 30

      "Albatross", 2.99, "On a stick!"
      "Crunchy Frog", 1.49, "If we took the bones out, it wouldn't be
      crunchy, now would it?"
      "Gannet Ripple", 1.99, "On a stick!"


.. attention:: Test

.. caution:: Test

.. danger:: Test

.. error:: Test

.. hint:: Test

.. important:: Test

.. note:: Test

.. tip:: Test

.. warning:: Test

.. danger:: Beware killer rabbits!

.. note:: 密钥secret_key由接入方注册给美味不用等授权账号

.. warning:: Read the Docs developers do not support custom installs of our software. These documents are maintained by the community, and might not be up to date.





Lorem ipsum [#f1]_ dolor sit amet ... [#f2]_

.. rubric:: Footnotes

.. [#f1] Text of the first footnote.
.. [#f2] Text of the second footnote.

